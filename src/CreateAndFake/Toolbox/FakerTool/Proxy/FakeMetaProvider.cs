﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CreateAndFake.Toolbox.DuplicatorTool;

namespace CreateAndFake.Toolbox.FakerTool.Proxy
{
    /// <summary>Internal mechanism for faked object behavior.</summary>
    public sealed class FakeMetaProvider : IDuplicatable
    {
        /// <summary>Faked behavior.</summary>
        private readonly Stack<(CallData, Behavior)> _behavior = new();

        /// <summary>Record of calls made.</summary>
        private readonly IList<CallData> _log = new List<CallData>();

        /// <summary>Number of calls made with no associated behavior.</summary>
        private int _defaultCalls = 0;

        /// <summary>Mutatable for value tooling.</summary>
        public int Identifier { get; set; }

        /// <summary>Determines behavior when missing set behavior for a call.</summary>
        public bool ThrowByDefault { get; set; } = true;

        /// <summary>Starts up with a blank slate.</summary>
        public FakeMetaProvider() { }

        /// <summary>Copy constructor.</summary>
        /// <param name="behavior">Behavior to pass in.</param>
        /// <param name="log">Record of calls to pass in.</param>
        internal FakeMetaProvider(IEnumerable<(CallData, Behavior)> behavior, IEnumerable<CallData> log)
        {
            if (behavior == null) throw new ArgumentNullException(nameof(behavior));
            if (log == null) throw new ArgumentNullException(nameof(log));

            foreach ((CallData, Behavior) set in behavior)
            {
                _behavior.Push(set);
            }
            foreach (CallData data in log)
            {
                _log.Add(data);
            }
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <param name="duplicator">Duplicator to clone child values.</param>
        /// <returns>Clone that is equal in value to the instance.</returns>
        public IDuplicatable DeepClone(IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return new FakeMetaProvider(
                _behavior.Reverse().Select(t => duplicator.Copy(t)),
                _log.Select(t => duplicator.Copy(t)))
            {
                Identifier = Identifier,
                ThrowByDefault = ThrowByDefault,
                _defaultCalls = _defaultCalls
            };
        }

        /// <summary>Sets up behavior for th fake.</summary>
        /// <param name="callData">Call to set behavior for.</param>
        /// <param name="behavior">Behavior to tie to the call.</param>
        internal void SetCallBehavior(CallData callData, Behavior behavior)
        {
            if (callData == null) throw new ArgumentNullException(nameof(callData));
            if (behavior == null) throw new ArgumentNullException(nameof(behavior));

            _behavior.Push((callData, behavior));
        }

        /// <summary>Verifies behavior with associated times were called as expected.</summary>
        internal void Verify()
        {
            (CallData, Behavior)[] invalids = _behavior.Where(t => !t.Item2.HasExpectedCalls()).ToArray();
            if (invalids.Any())
            {
                throw new FakeVerifyException(invalids, _log);
            }
        }

        /// <summary>Verifies the number of calls made.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="callData">Call to verify.</param>
        internal void Verify(Times times, CallData callData)
        {
            if (times == null) throw new ArgumentNullException(nameof(times));
            if (callData == null) throw new ArgumentNullException(nameof(callData));

            IEnumerable<CallData> calls = _log.Where(c => callData.MatchesCall(c)).ToArray();
            if (!times.IsInRange(calls.Count()))
            {
                throw new FakeVerifyException(callData, times, calls.Count(), _log);
            }
        }

        /// <summary>Verifies the total number of calls made.</summary>
        /// <param name="times">Expected total.</param>
        internal void VerifyTotalCalls(Times times)
        {
            if (times == null) throw new ArgumentNullException(nameof(times));

            if (!times.IsInRange(_log.Count))
            {
                throw new FakeVerifyException(times, _log);
            }
        }

        /// <summary>Manager for all action calls.</summary>
        /// <param name="instance">The faked object.</param>
        /// <param name="name">Name of the method being called.</param>
        /// <param name="generics">Generics tied to the call.</param>
        /// <param name="args">Provided args to the call.</param>
        internal void CallVoid(object instance, string name, Type[] generics, object[] args)
        {
            object result = CallRet<object>(instance, name, generics, args);
            if (result != null)
            {
                throw new InvalidOperationException(
                    $"Method '{name}' expected void but instead returned '{result}'.");
            }
        }

        /// <summary>Manager for all func calls.</summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="instance">The faked object.</param>
        /// <param name="name">Name of the method being called.</param>
        /// <param name="generics">Generics tied to the call.</param>
        /// <param name="args">Provided args to the call.</param>
        /// <returns>Faked result previously set up.</returns>
        internal T CallRet<T>(object instance, string name, Type[] generics, object[] args)
        {
            CallData data = new(name, generics, args, null);
            _log.Add(data);

            (CallData, Behavior) match = _behavior.FirstOrDefault(t => t.Item1.MatchesCall(data));
            if (match.Equals(default))
            {
                _defaultCalls++;
                if (ThrowByDefault)
                {
                    throw new FakeCallException(data, _behavior.Select(b => b.Item1));
                }
                else
                {
                    return default;
                }
            }

            if (match.Item2.BaseCallType != null)
            {
                return CallBase<T>(instance, name, match, args);
            }
            else
            {
                return (T)match.Item2.Invoke(args);
            }
        }

        /// <summary>Calls the base method for a behavior.</summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="instance">The faked object.</param>
        /// <param name="name">Name of the method being called.</param>
        /// <param name="match">Behavior details.</param>
        /// <param name="args">Provided args to the call.</param>
        /// <returns>Base method result.</returns>
        private static T CallBase<T>(object instance, string name, (CallData, Behavior) match, object[] args)
        {
            MethodInfo method = match.Item2.BaseCallType.GetMethod(name);
            if (method == null)
            {
                throw new MissingMethodException($"Method '{name}' does not exist on '{match.Item2.BaseCallType}'");
            }
            else if (method.IsAbstract)
            {
                throw new InvalidOperationException($"Cannot call base '{name}' as it's abstract.");
            }
            else
            {
                Delegate caller = (Delegate)Activator.CreateInstance(
                    FindDelegateType(method), instance, method.MethodHandle.GetFunctionPointer());

                return (T)match.Item2.Invoke(caller, args);
            }
        }

        /// <summary>Matches a delegate to the method.</summary>
        /// <param name="methodInfo">Method to call.</param>
        /// <returns>Found delegate type.</returns>
        private static Type FindDelegateType(MethodInfo methodInfo)
        {
            IEnumerable<Type> args = methodInfo.GetParameters().Select(p => p.ParameterType);

            return (methodInfo.ReturnType == typeof(void))
                ? Expression.GetActionType(args.ToArray())
                : Expression.GetFuncType(args.Concat(new[] { methodInfo.ReturnType }).ToArray());
        }
    }
}
