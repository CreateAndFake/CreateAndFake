using System.Collections;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints;

/// <summary>Handles comparing <see cref="IEquatable{T}"/> instances for <see cref="IValuer"/>.</summary>
public sealed class EquatableCompareHint : CompareHint
{
    /// <inheritdoc/>
    protected override bool Supports(object? expected, object? actual, ValuerChainer valuer)
    {
        return expected != null
            && expected.GetType().Inherits(typeof(IEquatable<>).MakeGenericType(expected.GetType()))
            && expected is not IStructuralEquatable;
    }

    /// <inheritdoc/>
    protected override IEnumerable<Difference> Compare(object? expected, object? actual, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(expected, nameof(expected));

        if (!expected.Equals(actual))
        {
            return [new Difference(expected, actual)];
        }
        else
        {
            return [];
        }
    }

    /// <inheritdoc/>
    protected override int GetHashCode(object? item, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(item, nameof(item));

        return item.GetHashCode();
    }
}
