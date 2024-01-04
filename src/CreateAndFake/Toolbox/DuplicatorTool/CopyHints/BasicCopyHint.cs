using System;
using System.Runtime.Serialization;
using CreateAndFake.Design.Randomization;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

#pragma warning disable SYSLIB0050 // 'IObjectReference' is obsolete

/// <summary>Handles copying basic types for the duplicator.</summary>
public sealed class BasicCopyHint : CopyHint
{
    /// <inheritdoc/>
    protected internal sealed override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
    {
        Type type = source?.GetType();
        if (type == null
            || type.IsPrimitive
            || type.IsEnum
            || ValueRandom.ValueTypes.Contains(type)
            || type == typeof(object)
            || type == typeof(string)
            || type.Inherits<IObjectReference>())
        {
            return (true, source);
        }
        else
        {
            return (false, null);
        }
    }
}

#pragma warning restore SYSLIB0050 // 'IObjectReference' is obsolete
