using System.Collections.Generic;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.ValuerTool;

/// <typeparam name="T">Type to handle.</typeparam>
/// <inheritdoc/>
public abstract class CompareHint<T> : CompareHint
{
    /// <inheritdoc/>
    protected sealed override bool Supports(object expected, object actual, ValuerChainer valuer)
    {
        ArgumentGuard.ThrowIfNull(expected, nameof(expected));
        ArgumentGuard.ThrowIfNull(actual, nameof(actual));
        ArgumentGuard.ThrowIfNull(valuer, nameof(valuer));

        return expected is T && actual is T;
    }

    /// <inheritdoc/>
    protected sealed override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
    {
        return Compare((T)expected, (T)actual, valuer);
    }

    /// <inheritdoc cref="Compare(object,object,ValuerChainer)"/>
    protected abstract IEnumerable<Difference> Compare(T expected, T actual, ValuerChainer valuer);

    /// <inheritdoc/>
    protected sealed override int GetHashCode(object item, ValuerChainer valuer)
    {
        return GetHashCode((T)item, valuer);
    }

    /// <inheritdoc cref="GetHashCode(object,ValuerChainer)"/>
    protected abstract int GetHashCode(T item, ValuerChainer valuer);
}
