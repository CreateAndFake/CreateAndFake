using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
#if NETSTANDARD
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

#pragma warning disable SYSLIB0050 // 'Type.IsSerializable' is obsolete

/// <summary>Handles copying serializables for the duplicator.</summary>
public sealed class SerializableCopyHint : CopyHint
{
    /// <summary>Scope used to search for inner types.</summary>
    private const BindingFlags _Scope
        = BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Instance;

    /// <inheritdoc/>
    protected internal override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
    {
        if (source == null)
        {
            return (true, null);
        }
        else if (source is ISerializable)
        {
#if NETSTANDARD // Backwards compatibility.
            if (source.GetType().IsSerializable)
            {
                BinaryFormatter binary = new();
                using MemoryStream memory = new();

                binary.Serialize(memory, source);
                _ = memory.Seek(0, SeekOrigin.Begin);
                return (true, binary.Deserialize(memory));
            }
#endif
            HashSet<object> knownData = [];
            FlattenData(source, knownData);

            DataContractSerializer serializer = new(source.GetType(), knownData.Select(d => d.GetType()).Distinct());

            using MemoryStream stream = new();

            serializer.WriteObject(stream, source);
            _ = stream.Seek(0, SeekOrigin.Begin);
            return (true, serializer.ReadObject(stream));
        }
        else
        {
            return (false, null);
        }
    }

    /// <summary>Finds used objects inside <paramref name="source"/></summary>
    /// <param name="source">Instance being serialized.</param>
    /// <param name="foundData">Set to populate with found data.</param>
    private static void FindInnerData(object source, HashSet<object> foundData)
    {
        Type type = source.GetType();

        foreach (PropertyInfo property in type.GetProperties(_Scope).Where(p => p.CanRead))
        {
            FlattenData(property.GetValue(source), foundData);
        }

        foreach (FieldInfo field in type.GetFields(_Scope))
        {
            FlattenData(field.GetValue(source), foundData);
        }
    }

    /// <summary>Finds data associated with <paramref name="source"/></summary>
    /// <param name="source">Instance being serialized.</param>
    /// <param name="foundData">Set to populate with found data.</param>
    private static void FlattenData(object source, HashSet<object> foundData)
    {
        if (source == null)
        {
            return;
        }
        else if (!foundData.Add(source) || source.GetType().Inherits<string>())
        {
            return;
        }
        else if (source is IDictionary map)
        {
            foreach (DictionaryEntry item in map)
            {
                FlattenData(item.Key, foundData);
                FlattenData(item.Value, foundData);
            }
        }
        else if (source is IEnumerable values)
        {
            IEnumerator gen = values.GetEnumerator();
            while (gen.MoveNext())
            {
                FlattenData(gen.Current, foundData);
            }
        }
        else if (!source.GetType().IsValueType)
        {
            FindInnerData(source, foundData);
        }
    }
}

#pragma warning restore SYSLIB0050 // 'Type.IsSerializable' is obsolete
