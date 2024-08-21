﻿using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles randomizing <c>Enum</c> values for <see cref="IRandomizer"/>.</summary>
public sealed class EnumCreateHint : CreateHint
{
    /// <inheritdoc/>
    protected internal override (bool, object?) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (type?.IsEnum ?? false)
        {
            return (true, randomizer.Gen.NextItem(Enum.GetValues(type).Cast<object>()));
        }
        else
        {
            return (false, null);
        }
    }
}
