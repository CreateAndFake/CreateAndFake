﻿using System.Collections;
using System.Collections.Generic;
using CreateAndFake.Design;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints;

/// <summary>Handles comparing collections for <see cref="IValuer"/>.</summary>
public sealed class EnumerableCompareHint : CompareHint<IEnumerable>
{
    /// <inheritdoc/>
    protected override IEnumerable<Difference> Compare(
        IEnumerable expected, IEnumerable actual, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(expected, nameof(expected));
        ArgumentGuard.ThrowIfNull(actual, nameof(actual));
        ArgumentGuard.ThrowIfNull(valuer, nameof(valuer));

        return LazyCompare(expected, actual, valuer);
    }

    /// <inheritdoc cref="Compare"/>
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

    /// <inheritdoc/>
    protected override int GetHashCode(IEnumerable item, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(item, nameof(item));
        ArgumentGuard.ThrowIfNull(valuer, nameof(valuer));

        int hash = ValueComparer.BaseHash;
        foreach (object value in item)
        {
            hash = hash * ValueComparer.HashMultiplier + valuer.GetHashCode(value);
        }
        return hash;
    }
}
