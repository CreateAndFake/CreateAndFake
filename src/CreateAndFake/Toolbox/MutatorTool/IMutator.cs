﻿namespace CreateAndFake.Toolbox.MutatorTool;

/// <summary>Changes the value of objects or creates alternatives.</summary>
public interface IMutator
{
    /// <typeparam name="T"><c>Type</c> to create.</typeparam>
    /// <inheritdoc cref="Variant"/>
    T Variant<T>(T instance, params T?[]? extraInstances);

    /// <summary>Creates an object with different values.</summary>
    /// <param name="type"><c>Type</c> to create.</param>
    /// <param name="instance">Object to diverge from.</param>
    /// <param name="extraInstances">Extra objects to diverge from.</param>
    /// <returns>
    ///     The created object that differs from <paramref name="instance"/> and <paramref name="extraInstances"/>.
    /// </returns>
    object Variant(Type type, object? instance, params object?[]? extraInstances);

    /// <typeparam name="T"><c>Type</c> to create.</typeparam>
    /// <inheritdoc cref="Unique"/>
    T Unique<T>(T instance, params T?[]? extraInstances);

    /// <summary>Creates an object with completely different values.</summary>
    /// <param name="type"><c>Type</c> to create.</param>
    /// <param name="instance">Object to diverge from.</param>
    /// <param name="extraInstances">Extra objects to diverge from.</param>
    /// <returns>
    ///     The created object that differs from <paramref name="instance"/> and <paramref name="extraInstances"/>.
    /// </returns>
    /// <remarks>Ignores types with too small of range for unique randomization.</remarks>
    object Unique(Type type, object? instance, params object?[]? extraInstances);

    /// <summary>Attempts to mutate an object.</summary>
    /// <param name="instance">Object to modify.</param>
    /// <returns><c>true</c> if <paramref name="instance"/> has been modified; <c>false</c> otherwise.</returns>
    bool Modify(object? instance);
}
