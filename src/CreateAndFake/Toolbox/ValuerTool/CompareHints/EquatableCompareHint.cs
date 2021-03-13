using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing equatables for <see cref="IValuer"/>.</summary>
    public sealed class EquatableCompareHint : CompareHint
    {
        /// <inheritdoc/>
        protected override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));

            return expected.GetType().Inherits(typeof(IEquatable<>).MakeGenericType(expected.GetType()))
                && !(expected is IStructuralEquatable);
        }

        /// <inheritdoc/>
        protected override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));

            if (!expected.Equals(actual))
            {
                return new[] { new Difference(expected, actual) };
            }
            else
            {
                return Enumerable.Empty<Difference>();
            }
        }

        /// <inheritdoc/>
        protected override int GetHashCode(object item, ValuerChainer valuer)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return item.GetHashCode();
        }
    }
}
