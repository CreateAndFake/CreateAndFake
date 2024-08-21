﻿using System.Reflection;

namespace CreateAndFake.Toolbox.FakerTool.Proxy;

/// <summary>Creates dynamic subclasses.</summary>
public static class Subclasser
{
    /// <summary>Assembly used to contain the dynamic types.</summary>
    public static AssemblyName AssemblyName => Emitter.AssemblyName;

    /// <summary>Cache of already created types.</summary>
    private static readonly Dictionary<TypeInfo, Type[]> _TypeCache = [];

    /// <summary>Determines if the type can be faked.</summary>
    /// <typeparam name="T">Type to check.</typeparam>
    /// <returns>True if possible; false otherwise.</returns>
    public static bool Supports<T>()
    {
        return Supports(typeof(T));
    }

    /// <summary>Determines if the type can be faked.</summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if possible; false otherwise.</returns>
    public static bool Supports(Type type)
    {
        return CanBeSubclassed(type).Item1;
    }

    /// <summary>Creates a subclass of the given type.</summary>
    /// <typeparam name="T">Parent type to inherit from.</typeparam>
    /// <param name="interfaces">Extra interfaces to implement.</param>
    /// <returns>An instance of the created child type.</returns>
    public static T Create<T>(params Type[] interfaces)
    {
        return (T)Create(typeof(T), interfaces);
    }

    /// <summary>Creates a subclass of the given type.</summary>
    /// <param name="parent">Parent type to inherit from.</param>
    /// <param name="interfaces">Extra interfaces to implement.</param>
    /// <returns>An instance of the created child type.</returns>
    public static IFaked Create(Type parent, params Type[] interfaces)
    {
        return (IFaked)CreateInfo(parent, interfaces).AsType()
            .GetConstructor([Emitter.MetaType])!
            .Invoke(new[] { new FakeMetaProvider() });
    }

    /// <summary>Creates a subclass of the given type.</summary>
    /// <param name="parent">Parent type to inherit from.</param>
    /// <param name="interfaces">Extra interfaces to implement.</param>
    /// <returns>The created child type.</returns>
    internal static TypeInfo CreateInfo(Type parent, params Type[] interfaces)
    {
        List<Type> allInterfaces = interfaces?.ToList() ?? [];
        allInterfaces.Add(Emitter.FakeType);

        Type realParent = parent ?? typeof(object);
        if (realParent.IsInterface)
        {
            allInterfaces.Add(realParent);
            realParent = typeof(object);
        }

        (bool, Exception?) possible = CanBeSubclassed(realParent);
        if (!possible.Item1)
        {
            throw possible.Item2!;
        }

        IEnumerable<Type> invalidInterfaces = allInterfaces.Where(t => !t.IsInterface);
        if (invalidInterfaces.Any())
        {
            throw new ArgumentException("Given interfaces not actually interfaces: '" +
                string.Join("', '", invalidInterfaces.Select(t => t.Name)) + "'.");
        }

        return FindOrBuildType(realParent, allInterfaces.Distinct().ToArray());
    }

    /// <summary>Determines if type can be subclassed.</summary>
    /// <param name="parent">Type to check.</param>
    /// <returns>True if possible; false if not with exception to throw.</returns>
    private static (bool, Exception?) CanBeSubclassed(Type parent)
    {
        if (parent == null)
        {
            return (true, null);
        }
        else if (parent.IsSealed)
        {
            return (false, new ArgumentException($"Cannot subclass the sealed type '{parent.Name}'."));
        }
        else if (parent.IsPointer)
        {
            return (false, new ArgumentException($"Cannot subclass the pointer '{parent.Name}'."));
        }
        else if (parent.ContainsGenericParameters)
        {
            return (false, new ArgumentException($"Cannot subclass with unspecified generics '{parent.Name}'."));
        }
        else if (!parent.IsVisibleTo(AssemblyName))
        {
            return (false, new ArgumentException($"Cannot subclass with nonpublic type '{parent.Name}'. "
                + $"Think about adding 'InternalsVisibleTo(\"{AssemblyName.Name}\")' to the type's assembly."));
        }
        else if (parent == typeof(Array))
        {
            return (false, new ArgumentException($"Cannot subclass system reserved '{nameof(Array)}' type."));
        }
        else
        {
            return (true, null);
        }
    }

    /// <summary>Finds or creates a type with the given hierarchy.</summary>
    /// <param name="parent">Parent type to inherit from.</param>
    /// <param name="interfaces">Extra interfaces to implement.</param>
    /// <returns>The cached or created child type.</returns>
    private static TypeInfo FindOrBuildType(Type parent, Type[] interfaces)
    {
        lock (_TypeCache)
        {
            TypeInfo cachedType = _TypeCache
                .Where(t => t.Key.BaseType == parent)
                .SingleOrDefault(t => !t.Value
                    .Except(interfaces)
                    .Union(interfaces
                        .Except(t.Value))
                    .Any()).Key;

            if (cachedType != null)
            {
                return cachedType;
            }
            else
            {
                TypeInfo newType = Emitter.BuildType(parent, interfaces);
                _TypeCache.Add(newType, interfaces);
                return newType;
            }
        }
    }
}
