using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing objects for the valuer.</summary>
    public sealed class StatelessCompareHint : CompareHint
    {
        /// <summary>Flags used to find properties and fields.</summary>
        private const BindingFlags _Scope = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>Determines if the objects are supported by the hint.</summary>
        /// <param name="expected">First object under question.</param>
        /// <param name="actual">Second object under question.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>True if the objects can be compared; false otherwise.</returns>
        protected override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));

            Type type = expected.GetType();
            return !type.GetProperties(_Scope).Any(p => p.CanRead)
                && !type.GetFields(_Scope).Any();
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
        {
            return Enumerable.Empty<Difference>();
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(object item, ValuerChainer valuer)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return item.GetType().GetHashCode();
        }
    }
}
