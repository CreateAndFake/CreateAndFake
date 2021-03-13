using System;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Handles generation of collection types for the randomizer.</summary>
    public abstract class CreateCollectionHint : CreateHint
    {
        /// <param name="type">Type to generate.</param>
        /// <param name="size">Number of items to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <inheritdoc cref="CreateHint.TryCreate(Type,RandomizerChainer)"/>
        protected internal abstract (bool, object) TryCreate(Type type, int size, RandomizerChainer randomizer);
    }
}
