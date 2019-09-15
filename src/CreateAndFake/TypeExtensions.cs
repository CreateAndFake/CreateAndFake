using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CreateAndFake.Toolbox
{
    /// <summary>Extensions for types.</summary>
    public static class TypeExtensions
    {
        /// <summary>Keeps track of type inheritance.</summary>
        private static readonly IDictionary<Type, HashSet<Type>> _ChildCache = new Dictionary<Type, HashSet<Type>>();

        /// <summary>Finds subclasses of a type in the type's assembly.</summary>
        /// <param name="type">Type to locate subclasses for.</param>
        /// <returns>Found createable subclasses.</returns>
        public static IEnumerable<Type> FindLocalSubclasses(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.Assembly.GetTypes()
                .Where(t => !t.IsAbstract)
                .Where(t => t.Inherits(type));
        }

        /// <summary>Finds subclasses of a type in all loaded assemblies.</summary>
        /// <param name="type">Type to locate subclasses for.</param>
        /// <returns>Found createable subclasses.</returns>
        public static IEnumerable<Type> FindLoadedSubclasses(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.ReflectionOnly)
                .Where(a => !a.IsDynamic)
                .SelectMany(a =>
                    {
                        // Ignore assemblies that can't load.
                        try
                        {
                            return a.GetExportedTypes();
                        }
                        catch (FileNotFoundException)
                        {
                            return Type.EmptyTypes;
                        }
                        catch (ReflectionTypeLoadException)
                        {
                            return Type.EmptyTypes;
                        }
                    })
                .Where(t => !t.IsAbstract)
                .Where(t => t.Inherits(type));
        }

        /// <summary>Determines if the class is visible to the given assembly.</summary>
        /// <param name="type">Type to check.</param>
        /// <param name="assembly">Name of the assembly.</param>
        /// <returns>True if visible; false otherwise.</returns>
        public static bool IsVisibleTo(this Type type, AssemblyName assembly)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            return type.IsVisible || type.Assembly
                .GetCustomAttributes<InternalsVisibleToAttribute>()
                .Any(a => a.AssemblyName == assembly.Name);
        }

        /// <summary>Attempts to get the root generic type.</summary>
        /// <param name="type">Type to cast.</param>
        /// <returns>Casted type if generic; null otherwise.</returns>
        public static Type AsGenericType(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return (type.IsGenericType ? type.GetGenericTypeDefinition() : null);
        }

        /// <summary>Checks for inheritance.</summary>
        /// <typeparam name="T">Type to determine if a child.</typeparam>
        /// <param name="parent">Type to determine if a parent.</param>
        /// <returns>True if inherited; false otherwise.</returns>
        public static bool Inherits<T>(this Type parent)
        {
            return Inherits(parent, typeof(T));
        }

        /// <summary>Checks for inheritance.</summary>
        /// <param name="parent">Type to determine if a parent.</param>
        /// <param name="child">Type to determine if a child.</param>
        /// <returns>True if inherited; false otherwise.</returns>
        public static bool Inherits(this Type parent, Type child)
        {
            return IsInheritedBy(child, parent);
        }

        /// <summary>Checks for inheritance.</summary>
        /// <typeparam name="T">Type to determine if a parent.</typeparam>
        /// <param name="child">Type to determine if a child.</param>
        /// <returns>True if inherited; false otherwise.</returns>
        public static bool IsInheritedBy<T>(this Type child)
        {
            return IsInheritedBy(child, typeof(T));
        }

        /// <summary>Checks for inheritance.</summary>
        /// <param name="child">Type to determine if a child.</param>
        /// <param name="parent">Type to determine if a parent.</param>
        /// <returns>True if inherited; false otherwise.</returns>
        public static bool IsInheritedBy(this Type child, Type parent)
        {
            if (!_ChildCache.ContainsKey(parent))
            {
                lock (_ChildCache)
                {
                    if (!_ChildCache.ContainsKey(parent))
                    {
                        _ChildCache[parent] = new HashSet<Type>(FindChildren(parent).Distinct());
                    }
                }
            }

            HashSet<Type> children = _ChildCache[parent];
            return children.Contains(child)
                || children.Contains(Nullable.GetUnderlyingType(child));
        }

        /// <summary>Finds all types the given type inherits.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>Found types.</returns>
        private static IEnumerable<Type> FindChildren(Type type)
        {
            if (type == null) yield break;

            yield return type;

            if (type.IsGenericType)
            {
                yield return type.GetGenericTypeDefinition();
            }

            foreach (Type child in type.GetInterfaces().SelectMany(t => FindChildren(t)))
            {
                yield return child;
            }

            foreach (Type child in FindChildren(type.BaseType))
            {
                yield return child;
            }
        }
    }
}
