using System.Collections;
using System.Text;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <summary>Handles assertion calls for collections.</summary>
/// <param name="gen">Core value random handler.</param>
/// <param name="valuer">Handles comparisons.</param>
/// <param name="collection">Collection to check.</param>
public abstract class AssertGroupBase<T>(IRandom gen, IValuer valuer, IEnumerable collection)
    : AssertObjectBase<T>(gen, valuer, collection) where T : AssertGroupBase<T>
{
    /// <summary>Collection to check.</summary>
    protected IEnumerable Collection { get; } = collection;

    /// <summary>Verifies the collection is empty.</summary>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If collection has elements.</exception>
    public virtual AssertChainer<T> IsEmpty(string details = null)
    {
        return HasCount(0, details);
    }

    /// <summary>Verifies the collection is not empty.</summary>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If collection is null or has no elements.</exception>
    public virtual AssertChainer<T> IsNotEmpty(string details = null)
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

    /// <summary>Verifies the collection has <paramref name="count"/> elements.</summary>
    /// <param name="count">Size to check for.</param>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If collection size does not match <paramref name="count"/>.</exception>
    public virtual AssertChainer<T> HasCount(int count, string details = null)
    {
        if (Collection == null)
        {
            throw new AssertException(
                $"Expected collection of '{count}' elements, but was 'null'.", details, Gen.InitialSeed);
        }

        StringBuilder contents = new();
        int i = 0;
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
