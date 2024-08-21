using System.Collections;

namespace CreateAndFake.Toolbox.ValuerTool;

/// <summary>Compares objects by value via reflection if needed.</summary>
public interface IValuer : IEqualityComparer<object>, IEqualityComparer
{
    /// <summary>Finds the differences between <paramref name="expected"/> and <paramref name="actual"/>.</summary>
    /// <param name="expected">Object to compare with <paramref name="actual"/>.</param>
    /// <param name="actual">Potentially different object to compare against <paramref name="expected"/>.</param>
    /// <returns>Found differences between <paramref name="expected"/> and <paramref name="actual"/>.</returns>
    /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
    /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
    IEnumerable<Difference> Compare(object? expected, object? actual);

    /// <summary>Determines if <paramref name="x"/> equals <paramref name="y"/> by value.</summary>
    /// <param name="x">Object to compare with <paramref name="y"/>.</param>
    /// <param name="y">Object to compare with <paramref name="x"/>.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="x"/> equals <paramref name="y"/> by value; <c>false</c> otherwise.
    /// </returns>
    /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
    /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
    new bool Equals(object? x, object? y);

    /// <summary>Computes an identifying hash code for <paramref name="item"/> based upon value.</summary>
    /// <param name="item">Object to generate a hash code for.</param>
    /// <returns>The value computed hash code for <paramref name="item"/>.</returns>
    /// <exception cref="NotSupportedException">If no hint supports hashing the object.</exception>
    /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
    new int GetHashCode(object? item);

    /// <summary>Computes an identifying hash code for <paramref name="items"/> based upon value.</summary>
    /// <param name="items">Bundled objects to generate a single value hash code for.</param>
    /// <returns>The value computed hash code for <paramref name="items"/>.</returns>
    /// <exception cref="NotSupportedException">If no hint supports hashing an object.</exception>
    /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
    int GetHashCode(params object?[]? items);

    /// <summary>Adds <paramref name="hint"/> to be used for comparisons. Last added takes precedence.</summary>
    /// <param name="hint">Hint to add.</param>
    /// <remarks>Should only be modified in module initializers.</remarks>
    void AddHint(CompareHint hint);
}
