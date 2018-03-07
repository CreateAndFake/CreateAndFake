using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CreateAndFake.Design.Randomization
{
    /// <summary>Provides the core functionality for generic randomization.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords",
        MessageId = "Next", Justification = "Matches the Random convention.")]
    public interface IRandom
    {
        /// <summary>Checks if the given type is supported for randomization.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        /// <returns>True if supported; false otherwise.</returns>
        bool Supports<T>() where T : struct, IComparable, IComparable<T>, IEquatable<T>;

        /// <summary>Checks if the given type is supported for randomization.</summary>
        /// <param name="type">Type to verify.</param>
        /// <returns>True if supported; false otherwise.</returns>
        bool Supports(Type type);

        /// <summary>Generates a random value of the given value type.</summary>
        /// <typeparam name="T">Value type to generate.</typeparam>
        /// <returns>The generated value.</returns>
        /// <exception cref="NotSupportedException">If the type isn't supported.</exception>
        T Next<T>() where T : struct, IComparable, IComparable<T>, IEquatable<T>;

        /// <summary>Generates a random value of the given value type.</summary>
        /// <param name="type">Value type to generate.</param>
        /// <returns>The generated value.</returns>
        /// <exception cref="NotSupportedException">If the type isn't supported.</exception>
        object Next(Type type);

        /// <summary>Generates a positive constrained value of the given value type.</summary>
        /// <typeparam name="T">Value type to generate.</typeparam>
        /// <param name="max">Positive exclusive upper boundary for the value.</param>
        /// <returns>The generated value.</returns>
        /// <exception cref="NotSupportedException">If the type isn't supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If max is less than or equal to 0.</exception>
        T Next<T>(T max) where T : struct, IComparable, IComparable<T>, IEquatable<T>;

        /// <summary>Generates a constrained value of the given value type.</summary>
        /// <typeparam name="T">Value type to generate.</typeparam>
        /// <param name="min">Inclusive lower boundary for the value.</param>
        /// <param name="max">Exclusive upper boundary for the value.</param>
        /// <returns>The generated value.</returns>
        /// <exception cref="NotSupportedException">If the type isn't supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If max is lower than min.</exception>
        T Next<T>(T min, T max) where T : struct, IComparable, IComparable<T>, IEquatable<T>;

        /// <summary>Picks a random item from a collection.</summary>
        /// <typeparam name="T">Type of items being picked.</typeparam>
        /// <param name="items">Collection of items to pick from.</param>
        /// <returns>The picked item.</returns>
        T NextItem<T>(IEnumerable<T> items);
    }
}
