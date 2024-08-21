using System.Collections;
using System.Text;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <summary>Handles common collection assertion calls.</summary>
/// <param name="collection"><inheritdoc cref="Collection" path="/summary"/></param>
/// <inheritdoc cref="AssertObjectBase{T}"/>
public abstract class AssertGroupBase<T>(IRandom gen, IValuer valuer, IEnumerable? collection)
    : AssertObjectBase<T>(gen, valuer, collection) where T : AssertGroupBase<T>
{
    /// <summary>Collection to run assertion checks with.</summary>
    protected IEnumerable? Collection { get; } = collection;

    /// <summary>Verifies <c>collection</c> is empty.</summary>
    /// <param name="details">Optional failure details to include.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If <c>collection</c> fails to match the expected behavior.</exception>
    public virtual AssertChainer<T> IsEmpty(string? details = null)
    {
        return HasCount(0, details);
    }

    /// <summary>Verifies <c>collection</c> is not empty.</summary>
    /// <inheritdoc cref="IsEmpty"/>
    public virtual AssertChainer<T> IsNotEmpty(string? details = null)
    {
        if (Collection == null)
        {
            throw new AssertException(
                $"Expected collection with elements, but was 'null'.", details, Gen.InitialSeed);
        }
        else if (!Collection.GetEnumerator().MoveNext())
        {
            throw new AssertException(
                "Expected collection with elements, but was empty.", details, Gen.InitialSeed);
        }
        else
        {
            return ToChainer();
        }
    }

    /// <summary>Verifies <c>collection</c> has <paramref name="count"/> elements.</summary>
    /// <param name="count">Size that the <c>collection</c> should be.</param>
    /// <inheritdoc cref="IsEmpty"/>
    public virtual AssertChainer<T> HasCount(int count, string? details = null)
    {
        if (Collection == null)
        {
            throw new AssertException(
                $"Expected collection of '{count}' elements, but was 'null'.", details, Gen.InitialSeed);
        }

        int i = 0;
        StringBuilder contents = new();
        for (IEnumerator data = Collection.GetEnumerator(); data.MoveNext(); i++)
        {
            _ = contents.Append('[').Append(i).Append("]:").Append(data.Current).AppendLine();
        }

        if (i != count)
        {
            throw new AssertException(
                $"Expected collection of '{count}' elements, but was '{i}'.",
                details, Gen.InitialSeed, contents.ToString());
        }

        return ToChainer();
    }
}
