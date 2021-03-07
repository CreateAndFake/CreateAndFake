using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.FakerTool.Proxy
{
    /// <summary>Method call details.</summary>
    internal sealed class CallData : IDuplicatable
    {
        /// <summary>Name tied to the call.</summary>
        private readonly string _methodName;

        /// <summary>Generics tied to the call.</summary>
        private readonly Type[] _generics;

        /// <summary>Args tied to the call.</summary>
        private readonly object[] _args;

        /// <summary>How to compare call data.</summary>
        private readonly IValuer _valuer;

        /// <summary>Initializes a new instance of the <see cref="CallData"/> class.</summary>
        /// <param name="methodName">Name tied to the call.</param>
        /// <param name="generics">Generics tied to the call.</param>
        /// <param name="args">Args tied to the call.</param>
        /// <param name="valuer">How to compare call data.</param>
        internal CallData(string methodName, Type[] generics, object[] args, IValuer valuer)
        {
            _methodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            _generics = generics ?? throw new ArgumentNullException(nameof(generics));
            _args = args ?? throw new ArgumentNullException(nameof(args));
            _valuer = valuer;
        }

        /// <summary>Converts arg values to designated Arg matchers.</summary>
        /// <param name="argChanges">Created Args to substitue on the given values.</param>
        internal void ConvertArgs(Tuple<Arg, object>[] argChanges)
        {
            List<Tuple<Arg, object>> changes = argChanges.ToList();
            for (int i = 0; i < _args.Length; i++)
            {
                Tuple<Arg, object> change = changes.FirstOrDefault(c => c.Item2 == _args[i]);
                if (change != default)
                {
                    _args[i] = change.Item1;
                    _ = changes.Remove(change);
                }
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

            return new CallData(_methodName, _generics.ToArray(), duplicator.Copy(_args), duplicator.Copy(_valuer));
        }

        /// <summary>Determines if behavior is intended for a call.</summary>
        /// <param name="input">Details of the call.</param>
        /// <returns>True if matched; false otherwise.</returns>
        internal bool MatchesCall(CallData input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            return (_methodName == input._methodName)
                && GenericsMatch(input._generics)
                && ArgsMatch(input._args);
        }

        /// <summary>Determines if the call generics match the expected ones.</summary>
        /// <param name="inputGenerics">Generics used in the call.</param>
        /// <returns>True if matched; false otherwise.</returns>
        private bool GenericsMatch(Type[] inputGenerics)
        {
            bool matches = (inputGenerics.Length == _generics.Length);

            for (int i = 0; matches && i < inputGenerics.Length; i++)
            {
                if (_generics[i] != typeof(AnyGeneric))
                {
                    matches &= _generics[i] == inputGenerics[i];
                }
            }
            return matches;
        }

        /// <summary>Determines if the call args match the expected ones.</summary>
        /// <param name="inputArgs">Args used in the call.</param>
        /// <returns>True if matched; false otherwise.</returns>
        private bool ArgsMatch(object[] inputArgs)
        {
            bool matches = (inputArgs.Length == _args.Length);

            for (int i = 0; matches && i < inputArgs.Length; i++)
            {
                if (_args[i] is Arg exp)
                {
                    matches &= exp.Matches(inputArgs[i]);
                }
                else if (_valuer != null)
                {
                    matches &= _valuer.Equals(inputArgs[i], _args[i]);
                }
                else
                {
                    matches &= Equals(inputArgs[i], _args[i]);
                }
            }
            return matches;
        }

        /// <returns>String representation of the call.</returns>
        public override string ToString()
        {
            string gen = (_generics.Any()
                ? "<" + string.Join(", ", _generics.Select(g => g.Name)) + ">"
                : "");
            return _methodName + gen + "(" + string.Join(", ", _args.Select(i => i ?? "'null'")) + ")";
        }
    }
}
