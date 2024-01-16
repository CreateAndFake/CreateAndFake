using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CreateAndFake.Design.Content;

/// <summary>Compares objects by value.</summary>
/// <typeparam name="T">Instance type to compare.</typeparam>
public sealed class ValueComparer<T> : IComparer<T>, IEqualityComparer<T> where T : IValueEquatable
{
    /// <summary>Default instance for use.</summary>
    [SuppressMessage("Microsoft.Design",
        "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
        Justification = "Per generic intended.")]
    public static ValueComparer<T> Use { get; } = new ValueComparer<T>();

    /// <inheritdoc cref="ValueComparer.Equals(object,object)"/>
    public bool Equals(T x, T y)
    {
        return ValueComparer.Use.Equals(x, y);
    }

    /// <inheritdoc cref="ValueComparer.GetHashCode(object)"/>
    public int GetHashCode(T obj)
    {
        return ValueComparer.Use.GetHashCode(obj);
    }

    /// <inheritdoc cref="ValueComparer.Compare(object,object)"/>
    public int Compare(T x, T y)
    {
        return ValueComparer.Use.Compare(x, y);
    }
}
