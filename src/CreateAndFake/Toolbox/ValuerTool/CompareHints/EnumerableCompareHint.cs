using System;
using System.Collections;
using System.Collections.Generic;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing collections for the valuer.</summary>
    public sealed class EnumerableCompareHint : CompareHint<IEnumerable>
    {
        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(
            IEnumerable expected, IEnumerable actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return LazyCompare(expected, actual, valuer);
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        private static IEnumerable<Difference> LazyCompare(
            IEnumerable expected, IEnumerable actual, ValuerChainer valuer)
        {
            IEnumerator expectedEnumerator = expected.GetEnumerator();
            IEnumerator actualEnumerator = actual.GetEnumerator();
            int index = 0;

            while (expectedEnumerator.MoveNext())
            {
                if (actualEnumerator.MoveNext())
                {
                    foreach (Difference diff in valuer.Compare(expectedEnumerator.Current, actualEnumerator.Current))
                    {
                        yield return new Difference(index, diff);
                    }
                }
                else
                {
                    yield return new Difference(index, new Difference(expectedEnumerator.Current, "'outofbounds'"));
                }
                index++;
            }
            while (actualEnumerator.MoveNext())
            {
                yield return new Difference(index++, new Difference("'outofbounds'", actualEnumerator.Current));
            }
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(IEnumerable item, ValuerChainer valuer)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            int hash = ValueComparer.BaseHash;
            foreach (object value in item)
            {
                hash = hash * ValueComparer.HashMultiplier + valuer.GetHashCode(value);
            }
            return hash;
        }
    }
}
