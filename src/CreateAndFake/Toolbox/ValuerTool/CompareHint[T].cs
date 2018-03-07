using System;
using System.Collections.Generic;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Handles comparing specific types for the valuer.</summary>
    /// <typeparam name="T">Type to handle.</typeparam>
    public abstract class CompareHint<T> : CompareHint
    {
        /// <summary>Determines if the objects are supported by the hint.</summary>
        /// <param name="expected">First object under question.</param>
        /// <param name="actual">Second object under question.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>True if the objects can be compared; false otherwise.</returns>
        protected override sealed bool Supports(object expected, object actual, IValuer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return (expected is T && actual is T);
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>Found differences.</returns>
        protected override sealed IEnumerable<Difference> Compare(object expected, object actual, IValuer valuer)
        {
            return Compare((T)expected, (T)actual, valuer);
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>The generated hash.</returns>
        protected override sealed int GetHashCode(object item, IValuer valuer)
        {
            return GetHashCode((T)item, valuer);
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>Found differences.</returns>
        protected abstract IEnumerable<Difference> Compare(T expected, T actual, IValuer valuer);

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>The generated hash.</returns>
        protected abstract int GetHashCode(T item, IValuer valuer);
    }
}
