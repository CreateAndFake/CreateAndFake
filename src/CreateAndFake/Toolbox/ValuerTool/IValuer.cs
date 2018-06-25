using System;
using System.Collections;
using System.Collections.Generic;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Compares objects by value.</summary>
    public interface IValuer : IEqualityComparer<object>, IEqualityComparer
    {
        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <returns>Found differences.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        IEnumerable<Difference> Compare(object expected, object actual);

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if equal; false otherwise.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        new bool Equals(object x, object y);

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing the object.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        new int GetHashCode(object item);

        /// <summary>Returns a hash code for the specified objects.</summary>
        /// <param name="items">Objects to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing an object.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        int GetHashCode(params object[] items);
    }
}
