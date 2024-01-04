using System;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles generation of value types for the randomizer.</summary>
public sealed class ValueCreateHint : CreateHint
{
    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (randomizer.Gen.Supports(type))
        {
            return (true, randomizer.Gen.Next(type));
        }
        else
        {
            return (false, null);
        }
    }
}
