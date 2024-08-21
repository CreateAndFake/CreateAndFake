using System.Reflection;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles randomizing <see cref="IAsyncEnumerable{T}"/> collections for <see cref="IRandomizer"/>.</summary>
/// <param name="listHint">Handles creating internal representations.</param>
public sealed class AsyncCollectionCreateHint(CollectionCreateHint listHint) : CreateCollectionHint
{
    /// <inheritdoc cref="AsyncCollectionCreateHint"/>
    /// <inheritdoc cref="CollectionCreateHint"/> 
    public AsyncCollectionCreateHint(int minSize = 1, int range = 3)
        : this(new CollectionCreateHint(minSize, range)) { }

    /// <inheritdoc/>
    protected internal override (bool, object?) TryCreate(Type type, RandomizerChainer randomizer)
    {
        return TryCreate(type, () => listHint.TryCreate(
            typeof(List<>).MakeGenericType(type.GetGenericArguments().Single()), randomizer).Item2);
    }

    /// <inheritdoc/>
    protected internal override (bool, object?) TryCreate(Type type, int size, RandomizerChainer randomizer)
    {
        return TryCreate(type, () => listHint.TryCreate(
            typeof(List<>).MakeGenericType(type.GetGenericArguments().Single()), size, randomizer).Item2);
    }

    /// <param name="listMaker">Creates the backing data.</param>
    /// <inheritdoc cref="TryCreate(Type, RandomizerChainer)"/>
    private (bool, object?) TryCreate(Type type, Func<object?> listMaker)
    {
        if (type.Inherits(typeof(IAsyncEnumerable<>)))
        {
            return (true, GetType()
                .GetMethod(nameof(GetItems), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(type.GetGenericArguments().Single())
                .Invoke(null, [listMaker()]));
        }
        else
        {
            return (false, null);
        }
    }

    /// <summary>Supplies collection items asynchronously.</summary>
    /// <typeparam name="T">Item <c>Type</c> to supply.</typeparam>
    /// <param name="backing">Collection items to supply.</param>
    /// <returns>The collection made from <paramref name="backing"/>.</returns>
    private static async IAsyncEnumerable<T> GetItems<T>(List<T> backing)
    {
        for (int i = 0; i < backing.Count; i++)
        {
            await Task.Delay(0).ConfigureAwait(false);
            yield return backing[i];
        }
    }
}