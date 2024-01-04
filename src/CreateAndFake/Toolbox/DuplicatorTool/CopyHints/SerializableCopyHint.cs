using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

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

        if (source is ISerializable && source.GetType().IsSerializable)
        {
            DataContractSerializer serializer = new(source.GetType(), FindInnerTypes(source)
                .Where(t => t != null)
                .Distinct());

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

    private static IEnumerable<Type> FindInnerTypes(object source)
    {
        Type type = source.GetType();

        foreach (PropertyInfo property in type.GetProperties(_Scope).Where(p => p.CanRead))
        {
            yield return property.GetValue(source)?.GetType();
        }

        foreach (FieldInfo field in type.GetFields(_Scope))
        {
            yield return field.GetValue(source)?.GetType();
        }
    }
}

#pragma warning restore SYSLIB0050 // 'Type.IsSerializable' is obsolete
