using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying collections for the duplicator.</summary>
    public sealed class CollectionCopyHint : CopyHint<IEnumerable>
    {
        /// <summary>Special cases where the data needs to be reversed.</summary>
        private static Type[] s_ReverseCases = new[]
        {
            typeof(ConcurrentStack<>),
            typeof(Stack<>),
            typeof(Stack)
        };

        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Duplicate object.</returns>
        protected override IEnumerable Copy(IEnumerable source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return source;

            Type type = source.GetType();
            if (type.IsArray)
            {
                return CopyContents(source, duplicator);
            }
#if NETSTANDARD // Constructor missing in .NET full.
            else if (type.AsGenericType() == typeof(Dictionary<,>))
            {
                dynamic result = Activator.CreateInstance(type);
                foreach (dynamic item in CopyContents(source, duplicator))
                {
                    result.Add(item.Key, item.Value);
                }
                return result;
            }
#endif
            else
            {
                return (IEnumerable)Activator.CreateInstance(type, CopyContents(source, duplicator));
            }
        }

        /// <summary>Copies the contents of a collection.</summary>
        /// <param name="source">Collection with contents to copy.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Duplicate object.</returns>
        private static IEnumerable CopyContents(IEnumerable source, DuplicatorChainer duplicator)
        {
            Type type = source.GetType();
            Type genericType = type.AsGenericType();

            object[] data;
            if (s_ReverseCases.Contains(genericType ?? type))
            {
                data = CopyContentsHelper(source, duplicator).Reverse().ToArray();
            }
            else
            {
                data = CopyContentsHelper(source, duplicator).ToArray();
            }

            if (genericType != null)
            {
                Type[] args = type.GetGenericArguments();
                Type itemType = (args.Length != 1)
                    ? typeof(KeyValuePair<,>).MakeGenericType(args)
                    : args.Single();

                return ArrayCast(itemType, data);
            }
            else if (type.IsArray)
            {
                return ArrayCast(type.GetElementType(), data);
            }
            else
            {
                return data;
            }
        }

        /// <summary>Converts an array to a specific type.</summary>
        /// <param name="elementType">Array type to create.</param>
        /// <param name="data">Array to convert.</param>
        /// <returns>Converted array.</returns>
        private static Array ArrayCast(Type elementType, object[] data)
        {
            Array result = Array.CreateInstance(elementType, data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                result.SetValue(data[i], i);
            }
            return result;
        }

        /// <summary>Copies the contents of a collection.</summary>
        /// <param name="source">Collection with contents to copy.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Duplicate object.</returns>
        private static IEnumerable<object> CopyContentsHelper(IEnumerable source, DuplicatorChainer duplicator)
        {
            IEnumerator enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return duplicator.Copy(enumerator.Current);
            }
        }
    }
}
