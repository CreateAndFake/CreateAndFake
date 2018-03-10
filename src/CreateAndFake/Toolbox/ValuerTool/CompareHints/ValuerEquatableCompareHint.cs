using System.Collections.Generic;
using System.Reflection;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing equatables for the valuer.</summary>
    public sealed class ValuerEquatableCompareHint : CompareHint<IValuerEquatable>
    {
        /// <summary>Compares equatables by value as well.</summary>
        private static readonly ObjectCompareHint s_NestedHint
            = new ObjectCompareHint(BindingFlags.Public | BindingFlags.Instance);

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(
            IValuerEquatable expected, IValuerEquatable actual, ValuerChainer valuer)
        {
            if (!expected.ValuesEqual(actual, valuer.Valuer))
            {
                yield return new Difference(".ValuesEqual", new Difference(true, false));

                (bool, IEnumerable<Difference>) byValues = s_NestedHint.TryCompare(expected, actual, valuer);
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
        protected override int GetHashCode(IValuerEquatable item, ValuerChainer valuer)
        {
            return item.GetValueHash(valuer.Valuer);
        }
    }
}
