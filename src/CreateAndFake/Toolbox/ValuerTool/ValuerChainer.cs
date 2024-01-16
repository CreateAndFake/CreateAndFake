using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.ValuerTool;

/// <summary>Provides a callback into <see cref="IValuer"/> to create child values.</summary>
/// <param name="valuer">Reference to the actual valuer.</param>
/// <param name="hasher">Callback to the valuer to handle child hashes.</param>
/// <param name="comparer">Callback to the valuer to handle child comparisons.</param>
public sealed class ValuerChainer(
    IValuer valuer,
    Func<object, ValuerChainer, int> hasher,
    Func<object, object, ValuerChainer, IEnumerable<Difference>> comparer)
{
    /// <summary>Callback to valuer to handle child hashes.</summary>
    private readonly Func<object, ValuerChainer, int> _hasher
        = hasher ?? throw new ArgumentNullException(nameof(hasher));

    /// <summary>Callback to valuer to handle child comparisons.</summary>
    private readonly Func<object, object, ValuerChainer, IEnumerable<Difference>> _comparer
        = comparer ?? throw new ArgumentNullException(nameof(comparer));

    /// <summary>History of hashes to match up references.</summary>
    private readonly Dictionary<int, object> _hashHistory = [];

    /// <summary>History of comparisons to match up references.</summary>
    private readonly ICollection<(int, int)> _compareHistory = new HashSet<(int, int)>();

    /// <summary>Reference to the actual valuer.</summary>
    internal IValuer Valuer { get; } = valuer ?? throw new ArgumentNullException(nameof(valuer));

    /// <inheritdoc cref="IValuer.Compare"/>
    public IEnumerable<Difference> Compare(object expected, object actual)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        if (!CanTrack(expected) || !CanTrack(actual))
        {
            return _comparer.Invoke(expected, actual, this);
        }

        (int, int) refHash = (RuntimeHelpers.GetHashCode(expected), RuntimeHelpers.GetHashCode(actual));
        if (_compareHistory.Contains(refHash))
        {
            return Enumerable.Empty<Difference>();
        }

        _compareHistory.Add(refHash);
        return _comparer.Invoke(expected, actual, this);
    }

    /// <inheritdoc cref="IValuer.Equals"/>
    public new bool Equals(object x, object y)
    {
        return !Compare(x, y).Any();
    }

    /// <inheritdoc cref="IValuer.GetHashCode(object)"/>
    public int GetHashCode(object item)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        if (!CanTrack(item))
        {
            return _hasher.Invoke(item, this);
        }

        int refHash = RuntimeHelpers.GetHashCode(item);
        if (_hashHistory.TryGetValue(refHash, out object stored)
            && ReferenceEquals(item, stored))
        {
            return 0;
        }

        _hashHistory[refHash] = item;

        int hash = _hasher.Invoke(item, this);
        _ = _hashHistory.Remove(refHash);
        return hash;
    }

    /// <inheritdoc cref="IValuer.GetHashCode(object[])"/>
    public int GetHashCode(params object[] items)
    {
        return GetHashCode((object)items);
    }

    /// <summary>If <paramref name="item"/> can be tracked in history.</summary>
    /// <param name="item">Item to check.</param>
    /// <returns>True if tracking <paramref name="item"/> is possible; false otherwise.</returns>
    private static bool CanTrack(object item)
    {
        return !(item == null || item is IFaked || item.GetType().IsValueType);
    }
}
