using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CreateAndFake.Design.Randomization;

/// <summary>Provides the core functionality for generic randomization.</summary>
[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords",
    MessageId = "Next", Justification = "Matches the Random convention.")]
public interface IRandom
{
    /// <summary>Initial seed used by the instance if there is one.</summary>
    int? InitialSeed { get; }

    /// <summary>Checks if <typeparamref name="T"/> is supported for randomization.</summary>
    /// <typeparam name="T">Type to verify.</typeparam>
    /// <returns><c>true</c> if <typeparamref name="T"/> is supported; <c>false</c> otherwise.</returns>
    bool Supports<T>() where T : struct, IComparable, IComparable<T>, IEquatable<T>;

    /// <summary>Checks if <paramref name="type"/> is supported for randomization.</summary>
    /// <param name="type">Type to verify.</param>
    /// <returns><c>true</c> if <paramref name="type"/> is supported; <c>false</c> otherwise.</returns>
    bool Supports(Type type);

    /// <summary>Generates a random <typeparamref name="T"/> value.</summary>
    /// <typeparam name="T">Value type to generate.</typeparam>
    /// <returns>The generated <typeparamref name="T"/> value.</returns>
    /// <exception cref="NotSupportedException">If <typeparamref name="T"/> isn't supported.</exception>
    T Next<T>() where T : struct, IComparable, IComparable<T>, IEquatable<T>;

    /// <summary>Generates a random value of type <paramref name="valueType"/>.</summary>
    /// <param name="valueType">Value type to generate.</param>
    /// <returns>The generated value of type <paramref name="valueType"/>.</returns>
    /// <exception cref="NotSupportedException">If <paramref name="valueType"/> isn't supported.</exception>
    object Next(Type valueType);

    /// <summary>Generates a positive constrained <typeparamref name="T"/> value.</summary>
    /// <typeparam name="T">Value type to generate.</typeparam>
    /// <param name="max">Positive exclusive upper boundary for the value.</param>
    /// <returns>The generated <typeparamref name="T"/> value.</returns>
    /// <exception cref="NotSupportedException">If <typeparamref name="T"/> isn't supported.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="max"/> &lt;= <c>0</c>.</exception>
    T Next<T>(T max) where T : struct, IComparable, IComparable<T>, IEquatable<T>;

    /// <summary>Generates a constrained <typeparamref name="T"/> value.</summary>
    /// <typeparam name="T">Value type to generate.</typeparam>
    /// <param name="min">Inclusive lower boundary for the value.</param>
    /// <param name="max">Exclusive upper boundary for the value.</param>
    /// <returns>The generated value.</returns>
    /// <exception cref="NotSupportedException">If <typeparamref name="T"/> isn't supported.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="max"/> &lt; <paramref name="min"/>.
    /// </exception>
    T Next<T>(T min, T max) where T : struct, IComparable, IComparable<T>, IEquatable<T>;

    /// <summary>Picks a random item from <paramref name="items"/>.</summary>
    /// <typeparam name="T">Type of items being picked from.</typeparam>
    /// <param name="items">Collection of items to pick from.</param>
    /// <returns>The picked item from <paramref name="items"/>.</returns>
    T NextItem<T>(IEnumerable<T> items);

    /// <inheritdoc cref="DataRandom"/>
    /// <returns>New generated data.</returns>
    DataRandom NextData();
}
