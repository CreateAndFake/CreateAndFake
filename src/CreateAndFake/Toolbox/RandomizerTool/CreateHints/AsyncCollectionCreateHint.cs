using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles generation of async collections for the randomizer.</summary>
/// <param name="listHint">Handles creating internal representation.</param>
public sealed class AsyncCollectionCreateHint(CollectionCreateHint listHint) : CreateCollectionHint
{
    /// <summary>Initializes a new instance of the <see cref="AsyncCollectionCreateHint"/> class.</summary>
    /// <param name="minSize">Min size for created collections.</param>
    /// <param name="range">Size variance for created collections.</param>
    /// <remarks>Specifies the size of generated collections.</remarks>
    public AsyncCollectionCreateHint(int minSize = 1, int range = 3)
        : this(new CollectionCreateHint(minSize, range)) { }

    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
    {
        return TryCreate(type, () => listHint.TryCreate(
            typeof(List<>).MakeGenericType(type.GetGenericArguments().Single()), randomizer).Item2);
    }

    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, int size, RandomizerChainer randomizer)
    {
        return TryCreate(type, () => listHint.TryCreate(
            typeof(List<>).MakeGenericType(type.GetGenericArguments().Single()), size, randomizer).Item2);
    }

    /// <inheritdoc cref="TryCreate(Type, RandomizerChainer)"/>
    /// <param name="type">Type to generate.</param>
    /// <param name="listMaker">Creates the backing data.</param>
    private (bool, object) TryCreate(Type type, Func<object> listMaker)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        if (type.Inherits(typeof(IAsyncEnumerable<>)))
        {
            return (true, GetType()
                .GetMethod(nameof(GetItems), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(type.GetGenericArguments().Single())
                .Invoke(null, [listMaker()]));
        }
        else
        {
            return (false, null);
        }
    }

    /// <summary>Generates items asynchronously.</summary>
    /// <typeparam name="T">Item type to generate.</typeparam>
    /// <param name="backing">Items to generate.</param>
    /// <returns>The generated items.</returns>
    private static async IAsyncEnumerable<T> GetItems<T>(List<T> backing)
    {
        for (int i = 0; i < backing.Count; i++)
        {
            await Task.Delay(0).ConfigureAwait(false);
            yield return backing[i];
        }
    }
}