using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing equatables for the valuer.</summary>
    public sealed class EquatableCompareHint : CompareHint
    {
        /// <summary>Determines if the objects are supported by the hint.</summary>
        /// <param name="expected">First object under question.</param>
        /// <param name="actual">Second object under question.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>True if the objects can be compared; false otherwise.</returns>
        protected override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));

            return expected.GetType().Inherits(typeof(IEquatable<>).MakeGenericType(expected.GetType()))
                && !(expected is IStructuralEquatable);
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
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

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(object item, ValuerChainer valuer)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return item.GetHashCode();
        }
    }
}
