using System;
using System.Collections.Generic;
using System.Reflection;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing equatables for <see cref="IValuer"/>.</summary>
    public sealed class ValueEquatableCompareHint : CompareHint<IValueEquatable>
    {
        /// <summary>Compares equatables by value as well.</summary>
        private static readonly ObjectCompareHint _NestedHint
            = new(BindingFlags.Public | BindingFlags.Instance);

        /// <inheritdoc/>
        protected override IEnumerable<Difference> Compare(
            IValueEquatable expected, IValueEquatable actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            return LazyCompare(expected, actual, valuer);
        }

        /// <inheritdoc cref="Compare"/>
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

        /// <inheritdoc/>
        protected override int GetHashCode(IValueEquatable item, ValuerChainer valuer)
        {
            return ValueComparer.Use.GetHashCode(item);
        }
    }
}
