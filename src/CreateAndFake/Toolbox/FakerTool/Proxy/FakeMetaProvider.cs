using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Toolbox.DuplicatorTool;

namespace CreateAndFake.Toolbox.FakerTool.Proxy
{
    /// <summary>Internal mechanism for faked object behavior.</summary>
    public sealed class FakeMetaProvider : IDuplicatable
    {
        /// <summary>Faked behavior.</summary>
        private readonly Stack<(CallData, Behavior)> m_Behavior = new Stack<(CallData, Behavior)>();

        /// <summary>Record of calls made.</summary>
        private readonly IList<CallData> m_Log = new List<CallData>();

        /// <summary>Number of calls made with no associated behavior.</summary>
        private int m_DefaultCalls = 0;

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
            foreach (var set in behavior)
            {
                m_Behavior.Push(set);
            }
            foreach (CallData data in log)
            {
                m_Log.Add(data);
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
                m_Behavior.Reverse().Select(t => duplicator.Copy(t)),
                m_Log.Select(t => duplicator.Copy(t)))
            {
                Identifier = Identifier,
                ThrowByDefault = ThrowByDefault,
                m_DefaultCalls = m_DefaultCalls
            };
        }

        /// <summary>Sets up behavior for th fake.</summary>
        /// <param name="callData">Call to set behavior for.</param>
        /// <param name="behavior">Behavior to tie to the call.</param>
        internal void SetCallBehavior(CallData callData, Behavior behavior)
        {
            if (callData == null) throw new ArgumentNullException(nameof(callData));
            if (behavior == null) throw new ArgumentNullException(nameof(behavior));

            m_Behavior.Push((callData, behavior));
        }

        /// <summary>Verifies behavior with associated times were called as expected.</summary>
        internal void Verify()
        {
            var invalids = m_Behavior.Where(t => !t.Item2.HasExpectedCalls()).ToArray();
            if (invalids.Any())
            {
                throw new FakeVerifyException(invalids, m_Log);
            }
        }

        /// <summary>Verifies the number of calls made.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="callData">Call to verify.</param>
        internal void Verify(Times times, CallData callData)
        {
            if (callData == null) throw new ArgumentNullException(nameof(callData));

            IEnumerable<CallData> calls = m_Log.Where(c => callData.MatchesCall(c)).ToArray();
            if (!times.IsInRange(calls.Count()))
            {
                throw new FakeVerifyException(callData, times, calls.Count(), m_Log);
            }
        }

        /// <summary>Verifies the total number of calls made.</summary>
        /// <param name="times">Expected total.</param>
        internal void VerifyTotalCalls(Times times)
        {
            if (!times.IsInRange(m_Log.Count))
            {
                throw new FakeVerifyException(times, m_Log);
            }
        }

        /// <summary>Manager for all action calls.</summary>
        /// <param name="name">Name of the method being called.</param>
        /// <param name="generics">Generics tied to the call.</param>
        /// <param name="args">Provided args to the call.</param>
        internal void CallVoid(string name, Type[] generics, object[] args)
        {
            object result = CallRet<object>(name, generics, args);
            if (result != null)
            {
                throw new InvalidOperationException(
                    $"Method '{name}' expected void but instead returned '{result}'.");
            }
        }

        /// <summary>Manager for all func calls.</summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="name">Name of the method being called.</param>
        /// <param name="generics">Generics tied to the call.</param>
        /// <param name="args">Provided args to the call.</param>
        /// <returns>Faked result previously set up.</returns>
        internal T CallRet<T>(string name, Type[] generics, object[] args)
        {
            CallData data = new CallData(name, generics, args, null);
            m_Log.Add(data);

            var match = m_Behavior.FirstOrDefault(t => t.Item1.MatchesCall(data));
            if (match.Equals(default))
            {
                m_DefaultCalls++;
                if (ThrowByDefault)
                {
                    throw new FakeCallException(data, m_Behavior.Select(b => b.Item1));
                }
                else
                {
                    return default;
                }
            }
            return (T)match.Item2.Invoke(args);
        }
    }
}
