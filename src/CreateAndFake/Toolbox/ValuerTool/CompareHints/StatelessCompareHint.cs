using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing objects for the valuer.</summary>
    public sealed class StatelessCompareHint : CompareHint
    {
        /// <summary>Flags used to find properties and fields.</summary>
        private const BindingFlags s_Scope = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>Determines if the objects are supported by the hint.</summary>
        /// <param name="expected">First object under question.</param>
        /// <param name="actual">Second object under question.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>True if the objects can be compared; false otherwise.</returns>
        protected override bool Supports(object expected, object actual, IValuer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            Type type = expected.GetType();
            return !type.GetProperties(s_Scope).Any(p => p.CanRead)
                && !type.GetFields(s_Scope).Any();
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(object expected, object actual, IValuer valuer)
        {
            return Enumerable.Empty<Difference>();
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(object item, IValuer valuer)
        {
            return item.GetType().GetHashCode();
        }
    }
}
