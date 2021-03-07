using System.Collections.Generic;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Handles comparing specific types for the valuer.</summary>
    public abstract class CompareHint
    {
        /// <summary>
        ///     Tries to finds the differences between <paramref name="expected"/> and <paramref name="actual"/>.
        /// </summary>
        /// <param name="expected">Base object to compare with.</param>
        /// <param name="actual">Potentially different object to compare with.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>
        ///     (<c>true</c>, found differences) if successful;
        ///     (<c>false</c>, <c>null</c>) otherwise.
        /// </returns>
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

        /// <summary>Tries to calculate a hash code for <paramref name="item"/> based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>
        ///     (<c>true</c>, generated hash) if successful;
        ///     (<c>false</c>, <c>0</c>) otherwise.
        /// </returns>
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

        /// <summary>
        ///     Determines if <paramref name="expected"/> or <paramref name="actual"/> are supported by the hint.
        /// </summary>
        /// <returns><c>true</c> if the objects can be compared; <c>false</c> otherwise.</returns>
        /// <inheritdoc cref="TryCompare"/>
        protected abstract bool Supports(object expected, object actual, ValuerChainer valuer);

        /// <summary>Finds the differences between <paramref name="expected"/> and <paramref name="actual"/>.</summary>
        /// <returns>The found differences.</returns>
        /// <inheritdoc cref="TryCompare"/>
        protected abstract IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer);

        /// <summary>Calculates a hash code for <paramref name="item"/> based upon value.</summary>
        /// <returns>The generated hash.</returns>
        /// <inheritdoc cref="TryGetHashCode"/>
        protected abstract int GetHashCode(object item, ValuerChainer valuer);
    }
}
