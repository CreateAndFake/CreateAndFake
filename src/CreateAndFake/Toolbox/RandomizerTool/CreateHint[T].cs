﻿using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool;

/// <typeparam name="T"><c>Type</c> being supported for randomization.</typeparam>
/// <inheritdoc/>
public abstract class CreateHint<T> : CreateHint
{
    /// <inheritdoc/>
    protected internal sealed override (bool, object?) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (type.IsInheritedBy<T>()
            && (type != typeof(object) || typeof(T) == typeof(object))
            && !randomizer.AlreadyCreated<T>())
        {
            return (true, Create(randomizer));
        }
        else
        {
            return (false, null);
        }
    }

    /// <summary>Creates a random <typeparamref name="T"/> instance.</summary>
    /// <param name="randomizer">Handles randomizing child values.</param>
    /// <returns>The created <typeparamref name="T"/> instance.</returns>
    protected abstract T Create(RandomizerChainer randomizer);
}
