using System;
using System.Collections.Generic;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles basic type compare issues for <see cref="IValuer"/>.</summary>
    public sealed class EarlyFailCompareHint : CompareHint
    {
        /// <inheritdoc/>
        protected override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            Type objectType = expected?.GetType();
            return objectType == null
                || actual == null
                || objectType != actual.GetType()
                || objectType.IsPrimitive
                || objectType.IsEnum
                || objectType == typeof(string)
                || objectType == typeof(object)
                || expected is Delegate
                || expected is Type;
        }

        /// <inheritdoc/>
        protected override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null && actual == null) yield break;

            if (expected == null || actual == null)
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

        /// <inheritdoc/>
        protected override int GetHashCode(object item, ValuerChainer valuer)
        {
            return ValueComparer.Use.GetHashCode(item);
        }
    }
}
