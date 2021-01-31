using System;
using System.Collections.Generic;
using System.Reflection;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing equatables for the valuer.</summary>
    public sealed class ValueEquatableCompareHint : CompareHint<IValueEquatable>
    {
        /// <summary>Compares equatables by value as well.</summary>
        private static readonly ObjectCompareHint _NestedHint
            = new(BindingFlags.Public | BindingFlags.Instance);

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(
            IValueEquatable expected, IValueEquatable actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return LazyCompare(expected, actual, valuer);
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        private static IEnumerable<Difference> LazyCompare(
            IValueEquatable expected, IValueEquatable actual, ValuerChainer valuer)
        {
            if (!expected.ValuesEqual(actual))
            {
                yield return new Difference(".ValuesEqual", new Difference(true, false));

                (bool, IEnumerable<Difference>) byValues = _NestedHint.TryCompare(expected, actual, valuer);
                if (byValues.Item1)
                {
                    foreach (Difference difference in byValues.Item2)
                    {
                        yield return difference;
                    }
                }
            }
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(IValueEquatable item, ValuerChainer valuer)
        {
            return ValueComparer.Use.GetHashCode(item);
        }
    }
}
