using System;
using System.Linq;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of value types for the randomizer.</summary>
    public sealed class EnumCreateHint : CreateHint
    {
        /// <inheritdoc/>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

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
}
