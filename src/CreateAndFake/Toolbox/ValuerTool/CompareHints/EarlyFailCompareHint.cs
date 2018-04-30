using System;
using System.Collections.Generic;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles basic type compare issues for the valuer.</summary>
    public sealed class EarlyFailCompareHint : CompareHint
    {
        /// <summary>Determines if the objects are supported by the hint.</summary>
        /// <param name="expected">First object under question.</param>
        /// <param name="actual">Second object under question.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>True if the objects can be compared; false otherwise.</returns>
        protected override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            Type objectType = expected?.GetType();
            return expected == null
                || actual == null
                || objectType != actual.GetType()
                || objectType.IsPrimitive
                || objectType.IsEnum
                || objectType == typeof(string)
                || objectType == typeof(object)
                || expected is Delegate
                || expected is Type;
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null && actual == null)
            {
                yield break;
            }
            else if (expected == null || actual == null)
            {
                yield return new Difference(expected, actual);
            }
            else if (expected.GetType() != actual.GetType())
            {
                yield return new Difference(expected.GetType(), actual.GetType());
            }
            else if (!expected.Equals(actual))
            {
                yield return new Difference(expected, actual);
            }
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(object item, ValuerChainer valuer)
        {
            return ValueComparer.Use.GetHashCode(item);
        }
    }
}
