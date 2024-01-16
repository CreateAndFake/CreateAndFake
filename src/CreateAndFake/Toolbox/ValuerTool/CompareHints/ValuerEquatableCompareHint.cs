﻿using System.Collections.Generic;
using System.Reflection;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints;

/// <summary>Handles comparing equatables for <see cref="IValuer"/>.</summary>
public sealed class ValuerEquatableCompareHint : CompareHint<IValuerEquatable>
{
    /// <summary>Compares equatables by value as well.</summary>
    private static readonly ObjectCompareHint _NestedHint
        = new(BindingFlags.Public | BindingFlags.Instance);

    /// <inheritdoc/>
    protected override IEnumerable<Difference> Compare(
        IValuerEquatable expected, IValuerEquatable actual, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(expected, nameof(expected));
        ArgumentGuard.ThrowIfNull(valuer, nameof(valuer));

        return LazyCompare(expected, actual, valuer);
    }

    /// <inheritdoc cref="Compare"/>
    private static IEnumerable<Difference> LazyCompare(
        IValuerEquatable expected, IValuerEquatable actual, ValuerChainer valuer)
    {
        if (!expected.ValuesEqual(actual, valuer.Valuer))
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
    protected override int GetHashCode(IValuerEquatable item, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(item, nameof(item));
        ArgumentGuard.ThrowIfNull(valuer, nameof(valuer));

        return item.GetValueHash(valuer.Valuer);
    }
}
