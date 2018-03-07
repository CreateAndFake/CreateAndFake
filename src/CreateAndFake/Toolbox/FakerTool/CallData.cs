using System;
using System.Linq;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Method call details.</summary>
    public sealed class CallData : IDuplicatable
    {
        /// <summary>Name tied to the call.</summary>
        private readonly string m_MethodName;

        /// <summary>Generics tied to the call.</summary>
        private readonly Type[] m_Generics;

        /// <summary>Args tied to the call.</summary>
        private readonly object[] m_Args;

        /// <summary>How to compare call data.</summary>
        private readonly IValuer m_Valuer;

        /// <summary>Populates the details.</summary>
        /// <param name="methodName">Name tied to the call.</param>
        /// <param name="generics">Generics tied to the call.</param>
        /// <param name="args">Args tied to the call.</param>
        /// <param name="valuer">How to compare call data.</param>
        public CallData(string methodName, Type[] generics, object[] args, IValuer valuer)
        {
            m_MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            m_Generics = generics ?? throw new ArgumentNullException(nameof(generics));
            m_Args = args ?? throw new ArgumentNullException(nameof(args));
            m_Valuer = valuer;
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

            return new CallData(m_MethodName, m_Generics.ToArray(), duplicator.Copy(m_Args), duplicator.Copy(m_Valuer));
        }

        /// <summary>Determines if behavior is intended for a call.</summary>
        /// <param name="input">Details of the call.</param>
        /// <returns>True if matched; false otherwise.</returns>
        internal bool MatchesCall(CallData input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            return (m_MethodName == input.m_MethodName)
                && m_Generics.SequenceEqual(input.m_Generics)
                && ArgsMatch(input.m_Args);
        }

        /// <summary>Determines if the call args match the expected ones.</summary>
        /// <param name="inputArgs">Args used in the call.</param>
        /// <returns>True if matched; false otherwise.</returns>
        private bool ArgsMatch(object[] inputArgs)
        {
            bool matches = (inputArgs.Length == m_Args.Length);

            for (int i = 0; matches && i < inputArgs.Length; i++)
            {
                if (m_Args[i] is Arg exp)
                {
                    matches &= exp.Matches(inputArgs[i]);
                }
                else if (m_Valuer != null)
                {
                    matches &= m_Valuer.Equals(inputArgs[i], m_Args[i]);
                }
                else
                {
                    matches &= Equals(inputArgs[i], m_Args[i]);
                }
            }
            return matches;
        }

        /// <returns>String representation of the call.</returns>
        public override string ToString()
        {
            string gen = (m_Generics.Any()
                ? "<" + string.Join(", ", m_Generics.Select(g => g.Name)) + ">"
                : "");
            return m_MethodName + gen + "(" + string.Join(", ", m_Args.Select(i => i ?? "'null'")) + ")";
        }
    }
}
