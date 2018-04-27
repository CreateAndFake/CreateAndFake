using System;
using System.Reflection;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Expresses a value difference between two objects.</summary>
    public sealed class Difference : IValueEquatable, IDeepCloneable
    {
        /// <summary>Message stating the difference.</summary>
        private readonly Lazy<string> m_Message;

        /// <summary>Details a difference of types.</summary>
        /// <param name="expected">Type of the first object being compared.</param>
        /// <param name="actual">Type of the second object being compared.</param>
        public Difference(Type expected, Type actual)
        {
            m_Message = new Lazy<string>(
                () => $" -> Expected type:<{expected}>, Actual type:<{actual}>");
        }

        /// <summary>Details a difference of values.</summary>
        /// <param name="expected">First value being compared.</param>
        /// <param name="actual">Second value being compared.</param>
        public Difference(object expected, object actual)
        {
            m_Message = new Lazy<string>(() => $" -> Expected:<{expected}>, Actual:<{actual}>");
        }

        /// <summary>Details a difference on a member.</summary>
        /// <param name="member">Member where the objects differed.</param>
        /// <param name="difference">Found difference.</param>
        public Difference(MemberInfo member, Difference difference)
            : this("." + member?.Name, difference)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
        }

        /// <summary>Details a difference on an index.</summary>
        /// <param name="index">Index where the objects differed.</param>
        /// <param name="difference">Found difference.</param>
        public Difference(int index, Difference difference)
            : this($"[{index}]", difference) { }

        /// <summary>Details a difference.</summary>
        /// <param name="access">Acess method where the objects differed.</param>
        /// <param name="difference">Found difference.</param>
        public Difference(string access, Difference difference)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (difference == null) throw new ArgumentNullException(nameof(difference));

            m_Message = new Lazy<string>(() => access + difference);
        }

        /// <summary>Details a difference.</summary>
        /// <param name="message">Message stating the difference.</param>
        public Difference(string message)
        {
            m_Message = new Lazy<string>(() => message);
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <returns>Clone that is equal in value to the current instance.</returns>
        public IDeepCloneable DeepClone()
        {
            return new Difference(m_Message.Value);
        }

        /// <summary>Compares by value.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public bool ValuesEqual(object other)
        {
            return other != null
                && GetType() == other.GetType()
                && m_Message.Value == ((Difference)other).m_Message.Value;
        }

        /// <summary>Generates a hash based upon value.</summary>
        /// <returns>The generated hash code.</returns>
        public int GetValueHash()
        {
            return ValueComparer.Use.GetHashCode(m_Message.Value);
        }

        /// <returns>String representation of the difference.</returns>
        public override string ToString()
        {
            return m_Message.Value;
        }
    }
}
