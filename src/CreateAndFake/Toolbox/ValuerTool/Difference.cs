using System;
using System.Reflection;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Expresses a value difference between two objects.</summary>
    public sealed class Difference : IValueEquatable, IDeepCloneable
    {
        /// <summary>Message stating the difference.</summary>
        private readonly Lazy<string> _message;

        /// <summary>Initializes a new instance of the <see cref="Difference"/> class.</summary>
        /// <param name="expectedType">Type of the compared expected object.</param>
        /// <param name="actualType">Type of the compared actual object.</param>
        public Difference(Type expectedType, Type actualType)
        {
            _message = new Lazy<string>(
                () => $" -> Expected type:<{expectedType}>, Actual type:<{actualType}>");
        }

        /// <summary>Initializes a new instance of the <see cref="Difference"/> class.</summary>
        /// <param name="expected">Object compared with <paramref name="actual"/>.</param>
        /// <param name="actual">Object compared against <paramref name="expected"/>.</param>
        public Difference(object expected, object actual)
        {
            _message = new Lazy<string>(() => $" -> Expected:<{expected}>, Actual:<{actual}>");
        }

        /// <summary>Initializes a new instance of the <see cref="Difference"/> class.</summary>
        /// <param name="member">Member where the compared objects differed.</param>
        /// <param name="difference">Found difference for the compared objects.</param>
        public Difference(MemberInfo member, Difference difference)
            : this("." + member?.Name, difference)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
        }

        /// <summary>Initializes a new instance of the <see cref="Difference"/> class.</summary>
        /// <param name="index">Index where the compared objects differed.</param>
        /// <param name="difference">Found difference for the compared objects.</param>
        public Difference(int index, Difference difference)
            : this($"[{index}]", difference) { }

        /// <summary>Initializes a new instance of the <see cref="Difference"/> class.</summary>
        /// <param name="access">Access method where the compared objects differed.</param>
        /// <param name="difference">Found difference for the compared objects.</param>
        public Difference(string access, Difference difference)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (difference == null) throw new ArgumentNullException(nameof(difference));

            _message = new Lazy<string>(() => access + difference.ToString());
        }

        /// <summary>Initializes a new instance of the <see cref="Difference"/> class.</summary>
        /// <param name="message">Message stating the difference.</param>
        public Difference(string message)
        {
            _message = new Lazy<string>(() => message);
        }

        /// <inheritdoc/>
        public IDeepCloneable DeepClone()
        {
            return new Difference(_message.Value);
        }

        /// <inheritdoc/>
        public bool ValuesEqual(object other)
        {
            return other != null
                && GetType() == other.GetType()
                && _message.Value == ((Difference)other)._message.Value;
        }

        /// <inheritdoc/>
        public int GetValueHash()
        {
            return ValueComparer.Use.GetHashCode(_message.Value);
        }

        /// <summary>Converts this object to a string.</summary>
        /// <returns>String representation of the difference.</returns>
        public override string ToString()
        {
            return _message.Value;
        }
    }
}
