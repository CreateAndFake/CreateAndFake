using System;
using System.Linq;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles generation of value types for the randomizer.</summary>
public sealed class EnumCreateHint : CreateHint
{
    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (type.IsEnum)
        {
            return (true, randomizer.Gen.NextItem(Enum.GetValues(type).Cast<object>()));
        }
        else
        {
            return (false, null);
        }
    }
}
