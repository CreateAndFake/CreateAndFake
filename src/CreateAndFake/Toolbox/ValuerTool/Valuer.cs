﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;

namespace CreateAndFake.Toolbox.ValuerTool;

/// <inheritdoc cref="IValuer"/>
/// <param name="includeDefaultHints">If the default set of hints should be used.</param>
/// <param name="hints"><inheritdoc cref="_hints" path="/summary"/></param>
public sealed class Valuer(bool includeDefaultHints = true, params CompareHint[]? hints) : IValuer, IDuplicatable
{
    /// <summary>Default set of hints to use for comparisons.</summary>
    private static readonly CompareHint[] _DefaultHints =
    [
        new EarlyFailCompareHint(),
        new FakedCompareHint(),
        new TaskCompareHint(),
        new ValueEquatableCompareHint(),
        new ValuerEquatableCompareHint(),
        new EquatableCompareHint(),
        new AsyncEnumerableCompareHint(),
        new StringDictionaryCompareHint(),
        new DictionaryCompareHint(),
        new EnumerableCompareHint(),
        new ObjectCompareHint(BindingFlags.Public | BindingFlags.Instance),
        new ObjectCompareHint(BindingFlags.NonPublic | BindingFlags.Instance),
        new StatelessCompareHint()
    ];

    /// <summary>Hints used to compare specific types.</summary>
    private readonly List<CompareHint> _hints = (hints ?? Enumerable.Empty<CompareHint>())
            .Concat(includeDefaultHints ? _DefaultHints : [])
            .ToList();

    /// <inheritdoc/>
    public new bool Equals(object? x, object? y)
    {
        return !Compare(x, y).Any();
    }

    /// <inheritdoc/>
    [SuppressMessage("Microsoft.Design",
        "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
        Justification = "Forwarded.")]
    public int GetHashCode(object? item)
    {
        string? typeName = item?.GetType().Name;
        try
        {
            return GetHashCode(item, new ValuerChainer(this, GetHashCode, Compare));
        }
        catch (InsufficientExecutionStackException)
        {
            throw new InsufficientExecutionStackException(
                $"Ran into infinite generation trying to hash type '{typeName}'.");
        }
    }

    /// <param name="chainer">Handles callback behavior for child values.</param>
    /// <inheritdoc cref="GetHashCode(object)"/>
    private int GetHashCode(object? item, ValuerChainer chainer)
    {
        (bool, int) result = _hints
            .Select(h => h.TryGetHashCode(item, chainer))
            .FirstOrDefault(r => r.Item1);

        if (!result.Equals(default))
        {
            return result.Item2;
        }
        else
        {
            throw new NotSupportedException(
                $"Type '{item?.GetType().FullName}' not supported by the valuer. " +
                "Create a hint to generate the type and pass it to the valuer.");
        }
    }

    /// <inheritdoc/>
    public int GetHashCode(params object?[]? items)
    {
        return GetHashCode((object?)items);
    }

    /// <inheritdoc/>
    public IEnumerable<Difference> Compare(object? expected, object? actual)
    {
        string? typeName = (expected ?? actual)?.GetType().Name;
        try
        {
            return Compare(expected, actual, new ValuerChainer(this, GetHashCode, Compare));
        }
        catch (InsufficientExecutionStackException)
        {
            throw new InsufficientExecutionStackException(
                $"Ran into infinite generation trying to compare type '{typeName}'.");
        }
    }

    /// <param name="chainer">Handles callback behavior for child values.</param>
    /// <inheritdoc cref="Compare(object,object)"/>
    private IEnumerable<Difference> Compare(object? expected, object? actual, ValuerChainer chainer)
    {
        if (ReferenceEquals(expected, actual))
        {
            return [];
        }

        (bool, IEnumerable<Difference>?) result = _hints
            .Select(h => h.TryCompare(expected, actual, chainer))
            .FirstOrDefault(r => r.Item1);

        if (!result.Equals(default))
        {
            return result.Item2!;
        }
        else
        {
            throw new NotSupportedException(
                $"Type '{expected?.GetType().FullName}' not supported by the valuer. " +
                "Create a hint to generate the type and pass it to the valuer.");
        }
    }

    /// <inheritdoc/>
    public IDuplicatable DeepClone(IDuplicator duplicator)
    {
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        return new Valuer(false, [.. duplicator.Copy(_hints)]);
    }

    /// <inheritdoc/>
    public void AddHint(CompareHint hint)
    {
        _hints.Insert(0, hint ?? throw new ArgumentNullException(nameof(hint)));
    }
}
