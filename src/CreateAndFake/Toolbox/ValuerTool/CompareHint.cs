using System.Collections.Generic;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Handles comparing specific types for the valuer.</summary>
    public abstract class CompareHint
    {
        /// <summary>Tries to finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>If the objects could be compared and any found differences.</returns>
        internal (bool, IEnumerable<Difference>) TryCompare(object expected, object actual, ValuerChainer valuer)
        {
            if (Supports(expected, actual, valuer))
            {
                return (true, Compare(expected, actual, valuer));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Tries to calculate a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>If the hash could be calculated and the generated hash.</returns>
        internal (bool, int) TryGetHashCode(object item, ValuerChainer valuer)
        {
            if (Supports(item, item, valuer))
            {
                return (true, GetHashCode(item, valuer));
            }
            else
            {
                return (false, default);
            }
        }

        /// <summary>Determines if the objects are supported by the hint.</summary>
        /// <param name="expected">First object under question.</param>
        /// <param name="actual">Second object under question.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>True if the objects can be compared; false otherwise.</returns>
        protected abstract bool Supports(object expected, object actual, ValuerChainer valuer);

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected abstract IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer);

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected abstract int GetHashCode(object item, ValuerChainer valuer);
    }
}
