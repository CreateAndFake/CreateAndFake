using System;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Handles generation of specific types for the randomizer.</summary>
    public abstract class CreateHint
    {
        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal abstract (bool, object) TryCreate(Type type, RandomizerChainer randomizer);
    }
}
