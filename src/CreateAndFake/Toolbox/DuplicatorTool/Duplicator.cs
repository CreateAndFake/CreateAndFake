using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

namespace CreateAndFake.Toolbox.DuplicatorTool;

/// <inheritdoc cref="IDuplicator"/>
public sealed class Duplicator : IDuplicator, IDuplicatable
{
    /// <summary>Default set of hints to use for copying.</summary>
    private static readonly CopyHint[] _DefaultHints =
    [
        new CommonSystemCopyHint(),
        new TaskCopyHint(),
        new DeepCloneableCopyHint(),
        new DuplicatableCopyHint(),
        new BasicCopyHint(),
        new AsyncCollectionCopyHint(),
        new LegacyCollectionCopyHint(),
        new CollectionCopyHint(),
        new CloneableCopyHint(),
        new SerializableCopyHint(),
        new ObjectCopyHint()
    ];

    /// <summary>Verifies duplicates are valid.</summary>
    private readonly Asserter _asserter;

    /// <summary>Hints used to copy specific types.</summary>
    private readonly List<CopyHint> _hints;

    /// <summary>Initializes a new instance of the <see cref="Duplicator"/> class.</summary>
    /// <param name="asserter">Verifies duplicates are valid.</param>
    /// <param name="includeDefaultHints">If the default set of hints should be added.</param>
    /// <param name="hints">Hints used to copy specific types.</param>
    public Duplicator(Asserter asserter, bool includeDefaultHints = true, params CopyHint[] hints)
    {
        _asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));

        IEnumerable<CopyHint> inputHints = hints ?? Enumerable.Empty<CopyHint>();
        _hints = includeDefaultHints
            ? inputHints.Concat(_DefaultHints).ToList()
            : inputHints.ToList();
    }

    /// <inheritdoc/>
    public T Copy<T>(T source)
    {
        try
        {
            T result = Copy(source, new DuplicatorChainer(this, Copy));
            _asserter.ValuesEqual(source, result,
                $"Type '{source?.GetType()}' did not clone properly. " +
                "Verify/create a hint to generate the type and pass it to the duplicator.");
            return result;
        }
        catch (InsufficientExecutionStackException)
        {
            throw new InsufficientExecutionStackException(
                $"Ran into infinite generation trying to duplicate type '{source.GetType().Name}'.");
        }
    }

    /// <summary>Deep clones <paramref name="source"/>.</summary>
    /// <typeparam name="T">Type being cloned.</typeparam>
    /// <param name="source">Object to clone.</param>
    /// <param name="chainer">Handles callback behavior for child values.</param>
    /// <returns>Clone of <paramref name="source"/>.</returns>
    /// <exception cref="NotSupportedException">If no hint supports cloning the object.</exception>
    private T Copy<T>(T source, DuplicatorChainer chainer)
    {
        if (source == null)
        {
            return default;
        }

        (bool, object) result = _hints
            .Select(h => h.TryCopy(source, chainer))
            .FirstOrDefault(r => r.Item1);

        if (!result.Equals(default))
        {
            return (T)result.Item2;
        }
        else
        {
            throw new NotSupportedException(
                $"Type '{source.GetType().FullName}' not supported by the duplicator. " +
                "Create a hint to generate the type and pass it to the duplicator.");
        }
    }

    /// <inheritdoc/>
    public IDuplicatable DeepClone(IDuplicator duplicator)
    {
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        return new Duplicator(duplicator.Copy(_asserter), false, [.. duplicator.Copy(_hints)]);
    }

    /// <inheritdoc/>
    public void AddHint(CopyHint hint)
    {
        _hints.Insert(0, hint ?? throw new ArgumentNullException(nameof(hint)));
    }
}
