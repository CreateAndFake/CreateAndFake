using System;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Handles generation of a specific type for the randomizer.</summary>
    /// <typeparam name="T">Type capable of being randomized.</typeparam>
    public abstract class CreateHint<T> : CreateHint
    {
        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal override sealed (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

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

        /// <summary>Creates a random instance of the type.</summary>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>Created instance.</returns>
        protected abstract T Create(RandomizerChainer randomizer);
    }
}
