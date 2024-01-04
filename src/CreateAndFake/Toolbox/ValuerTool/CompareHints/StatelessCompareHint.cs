﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints;

/// <summary>Handles comparing objects for <see cref="IValuer"/>.</summary>
public sealed class StatelessCompareHint : CompareHint
{
    /// <summary>Flags used to find properties and fields.</summary>
    private const BindingFlags _Scope = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    /// <inheritdoc/>
    protected override bool Supports(object expected, object actual, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(expected, nameof(expected));
        ArgumentGuard.ThrowIfNull(actual, nameof(actual));

        Type type = expected.GetType();
        return !type.GetProperties(_Scope).Any(p => p.CanRead)
            && type.GetFields(_Scope).Length == 0;
    }

    /// <inheritdoc/>
    protected override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
    {
        return Enumerable.Empty<Difference>();
    }

    /// <inheritdoc/>
    protected override int GetHashCode(object item, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(item, nameof(item));

        return item.GetType().GetHashCode();
    }
}
