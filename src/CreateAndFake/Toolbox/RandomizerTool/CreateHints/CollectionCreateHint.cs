﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles generation of collections for the randomizer.</summary>
/// <param name="minSize">Min size for created collections.</param>
/// <param name="range">Size variance for created collections.</param>
/// <remarks>Specifies the size of generated collections.</remarks>
public sealed class CollectionCreateHint(int minSize = 1, int range = 3) : CreateCollectionHint
{
    /// <summary>Collections able to be randomized.</summary>
    private static readonly Type[] _Collections =
    [
        typeof(List<>),
        typeof(Dictionary<,>),

        typeof(Queue<>),
        typeof(Stack<>),
        typeof(HashSet<>),
        typeof(LinkedList<>),
        typeof(ArraySegment<>),
        typeof(ConcurrentQueue<>),
        typeof(ConcurrentStack<>),
        typeof(ConcurrentDictionary<,>)
    ];

    /// <summary>Collections that the hint will create.</summary>
    internal static IEnumerable<Type> PotentialCollections => _Collections.Select(i => i);

    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
    {
        return TryCreate(type, minSize + randomizer?.Gen.Next(range) ?? 0, randomizer);
    }

    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, int size, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        Type itemType = GetItemType(type);
        if (itemType != null && FindMatches(type, itemType).Any())
        {
            return (true, Create(type, size, itemType, randomizer));
        }
        else
        {
            return (false, null);
        }
    }

    /// <summary>Creates a random instance of the given type.</summary>
    /// <param name="type">Type to generate.</param>
    /// <param name="size">Number of items to generate.</param>
    /// <param name="itemType">Item type to be contained in the collection.</param>
    /// <param name="randomizer">Handles callback behavior for child values.</param>
    /// <returns>Created instance.</returns>
    private static object Create(Type type, int size, Type itemType, RandomizerChainer randomizer)
    {
        Type collection = randomizer.Gen.NextItem(FindMatches(type, itemType));
        Type newType = MakeNewType(collection, itemType);

        Array internalData = CreateInternalData(itemType, size,
            t => randomizer.Create(t, randomizer.Parent));

        if (newType == typeof(Array) || newType == internalData.GetType())
        {
            return internalData;
        }
#if NETSTANDARD // Constructor missing in .NET full.
        else if (newType.AsGenericType() == typeof(Dictionary<,>))
        {
            dynamic result = Activator.CreateInstance(newType);
            foreach (dynamic item in internalData)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
#endif
        else
        {
            return Activator.CreateInstance(newType, internalData);
        }
    }

    /// <summary>Finds the type to be contained by a created collection.</summary>
    /// <param name="type">Type to potentially be randomized with a collection.</param>
    /// <returns>Null if not logical; type for the collection otherwise.</returns>
    private static Type GetItemType(Type type)
    {
        Type[] args = type.IsGenericType
            ? type.GetGenericArguments()
            : [];

        if (type.IsArray)
        {
            return type.GetElementType();
        }
        else if (type.IsGenericTypeDefinition)
        {
            return null;
        }
        else if (args.Length == 1)
        {
            return args[0];
        }
        else if (args.Length == 2)
        {
            return typeof(KeyValuePair<,>).MakeGenericType(args[0], args[1]);
        }
        else
        {
            return null;
        }
    }

    /// <summary>Finds potential collection matches for a type.</summary>
    /// <param name="type">Type to find matches for.</param>
    /// <param name="itemType">Item type to be contained in the collection.</param>
    /// <returns>All possible matches.</returns>
    private static IEnumerable<Type> FindMatches(Type type, Type itemType)
    {
        Type typeAsGeneric = type.IsGenericType
            ? type.GetGenericTypeDefinition()
            : type;

        if (type.IsArray
            || typeAsGeneric.IsInheritedBy(typeof(IList<>))
            || typeAsGeneric.IsInheritedBy(typeof(IList)))
        {
            yield return typeof(Array);
        }

        foreach (Type match in _Collections.Where(typeAsGeneric.IsInheritedBy))
        {
            if (!match.Inherits<IDictionary>() || itemType.Inherits(typeof(KeyValuePair<,>)))
            {
                yield return match;
            }
        }
    }

    /// <summary>Creates basic structures for the given type.</summary>
    /// <param name="itemType">Item type to be contained in the collection.</param>
    /// <param name="size">Number of items to generate.</param>
    /// <param name="randomizer">Callback to the randomizer to create child values.</param>
    /// <returns>Data populated with random values.</returns>
    private static Array CreateInternalData(Type itemType, int size, Func<Type, object> randomizer)
    {
        Array data = Array.CreateInstance(itemType, size);
        for (int i = 0; i < data.Length; i++)
        {
            data.SetValue(randomizer.Invoke(itemType), i);
        }
        return data;
    }

    /// <summary>Constructs the new type to make for the collection.</summary>
    /// <param name="collection">Matching collection with details.</param>
    /// <param name="itemType">Item type to be contained in the collection.</param>
    /// <returns>Type to create.</returns>
    private static Type MakeNewType(Type collection, Type itemType)
    {
        if (!collection.IsGenericType)
        {
            return collection;
        }
        else if (collection.Inherits<IDictionary>())
        {
            return collection.MakeGenericType(itemType.GetGenericArguments());
        }
        else
        {
            return collection.MakeGenericType(itemType);
        }
    }
}
