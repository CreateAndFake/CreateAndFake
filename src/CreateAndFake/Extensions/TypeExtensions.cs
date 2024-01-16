﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox;

/// <summary>Extensions for types.</summary>
public static class TypeExtensions
{
    /// <summary>Keeps track of type inheritance.</summary>
    private static readonly Dictionary<Type, HashSet<Type>> _ChildCache = [];

    /// <summary>Finds subclasses of <paramref name="type"/> in the <paramref name="type"/>'s assembly.</summary>
    /// <param name="type"><see cref="Type"/> to locate subclasses for.</param>
    /// <returns>The found creatable subclasses for <paramref name="type"/>.</returns>
    public static IEnumerable<Type> FindLocalSubclasses(this Type type)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        return type.Assembly.GetTypes()
            .Where(t => !t.IsAbstract)
            .Where(t => t.Inherits(type))
            .Where(t => IsVisibleTo(t, Assembly.GetCallingAssembly().GetName()));
    }

    /// <summary>Finds subclasses of <paramref name="type"/> in all loaded assemblies.</summary>
    /// <param name="type"><see cref="Type"/> to locate subclasses for.</param>
    /// <returns>The found creatable subclasses for <paramref name="type"/>.</returns>
    public static IEnumerable<Type> FindLoadedSubclasses(this Type type)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.ReflectionOnly)
            .Where(a => !a.IsDynamic)
            .SelectMany(FindLoadedTypes)
            .Where(t => !t.IsAbstract)
            .Where(t => t.Inherits(type));
    }

    /// <summary>Finds all types in <paramref name="assembly"/>.</summary>
    /// <param name="assembly"><see cref="Assembly"/> to load types from.</param>
    /// <returns>The found types if <paramref name="assembly"/> can load; none otherwise.</returns>
    internal static Type[] FindLoadedTypes(Assembly assembly)
    {
        try
        {
            return assembly?.GetExportedTypes() ?? Type.EmptyTypes;
        }
        catch (FileNotFoundException)
        {
            return Type.EmptyTypes;
        }
        catch (ReflectionTypeLoadException)
        {
            return Type.EmptyTypes;
        }
    }

    /// <summary>Determines if <paramref name="type"/> is visible to <paramref name="assembly"/>.</summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <param name="assembly">Name of the <see cref="Assembly"/>.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="type"/> is visible to
    ///     <paramref name="assembly"/>; <c>false</c> otherwise.
    /// </returns>
    public static bool IsVisibleTo(this Type type, AssemblyName assembly)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(assembly, nameof(assembly));

        return type.IsVisible || type.Assembly
            .GetCustomAttributes<InternalsVisibleToAttribute>()
            .Any(a => a.AssemblyName == assembly.Name);
    }

    /// <summary>Attempts to get the root generic type of <paramref name="type"/>.</summary>
    /// <param name="type"><see cref="Type"/> to cast.</param>
    /// <returns>The casted <paramref name="type"/> if generic; null otherwise.</returns>
    public static Type AsGenericType(this Type type)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        return type.IsGenericType ? type.GetGenericTypeDefinition() : null;
    }

    /// <summary>Checks if <paramref name="parent"/> inherits <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">Potential child <see cref="Type"/> of <paramref name="parent"/>.</typeparam>
    /// <param name="parent">Potential parent <see cref="Type"/> of <typeparamref name="T"/>.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="parent"/> inherits <typeparamref name="T"/>; <c>false</c> otherwise.
    /// </returns>
    public static bool Inherits<T>(this Type parent)
    {
        return Inherits(parent, typeof(T));
    }

    /// <summary>Checks if <paramref name="parent"/> inherits <paramref name="child"/>.</summary>
    /// <param name="parent">Potential parent <see cref="Type"/> of <paramref name="child"/>.</param>
    /// <param name="child">Potential child <see cref="Type"/> of <paramref name="parent"/>.</param>
    /// <returns>
    ///     <c>true</c> if <paramref name="parent"/> inherits <paramref name="child"/>; <c>false</c> otherwise.
    /// </returns>
    public static bool Inherits(this Type parent, Type child)
    {
        return IsInheritedBy(child, parent);
    }

    /// <summary>Checks if <typeparamref name="T"/> inherits <paramref name="child"/>.</summary>
    /// <typeparam name="T">Potential parent <see cref="Type"/> of <paramref name="child"/>.</typeparam>
    /// <param name="child">Potential child <see cref="Type"/> of <typeparamref name="T"/>.</param>
    /// <returns>
    ///     <c>true</c> if <typeparamref name="T"/> inherits <paramref name="child"/>; <c>false</c> otherwise.
    /// </returns>
    public static bool IsInheritedBy<T>(this Type child)
    {
        return IsInheritedBy(child, typeof(T));
    }

    /// <inheritdoc cref="Inherits"/>
    public static bool IsInheritedBy(this Type child, Type parent)
    {
        HashSet<Type> children;
        lock (_ChildCache)
        {
            if (!_ChildCache.TryGetValue(parent, out children))
            {
                _ChildCache[parent] = children = new HashSet<Type>(FindChildren(parent).Distinct());
            }
        }

        return children.Contains(child)
            || children.Contains(Nullable.GetUnderlyingType(child));
    }

    /// <summary>Finds all types <paramref name="type"/> inherits.</summary>
    /// <param name="type"><see cref="Type"/> to check.</param>
    /// <returns>The found types inherited by <paramref name="type"/>.</returns>
    private static IEnumerable<Type> FindChildren(Type type)
    {
        if (type == null)
        {
            yield break;
        }

        yield return type;

        if (type.IsGenericType)
        {
            yield return type.GetGenericTypeDefinition();
        }

        foreach (Type child in type.GetInterfaces().SelectMany(FindChildren))
        {
            yield return child;
        }

        foreach (Type child in FindChildren(type.BaseType))
        {
            yield return child;
        }
    }
}
