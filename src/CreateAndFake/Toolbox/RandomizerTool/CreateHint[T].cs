using System;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Handles generation of a specific type for the randomizer.</summary>
    /// <typeparam name="T">Type capable of being randomized.</typeparam>
    public abstract class CreateHint<T> : CreateHint
    {
        /// <inheritdoc/>
        protected internal sealed override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
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
        /// <returns>The created instance.</returns>
        protected abstract T Create(RandomizerChainer randomizer);
    }
}
