using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CreateAndFake.Design.Content
{
    /// <summary>Compares objects by value.</summary>
    /// <typeparam name="T">Instance type to compare.</typeparam>
    public sealed class ValueComparer<T> : IComparer<T>, IEqualityComparer<T> where T : IValueEquatable
    {
        /// <summary>Default instance for use.</summary>
        [SuppressMessage("Microsoft.Design",
            "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "Per generic intended.")]
        public static ValueComparer<T> Use { get; } = new ValueComparer<T>();

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public bool Equals(T x, T y)
        {
            return ValueComparer.Use.Equals(x, y);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">Object to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        public int GetHashCode(T obj)
        {
            return ValueComparer.Use.GetHashCode(obj);
        }

        /// <summary>Compares to sort values by their value hash.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Difference between value hashes.</returns>
        public int Compare(T x, T y)
        {
            return ValueComparer.Use.Compare(x, y);
        }
    }
}
