using System.Runtime.Serialization;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

#pragma warning disable SYSLIB0050 // 'IObjectReference' is obsolete

/// <summary>Handles cloning basic types for <see cref="IDuplicator"/> .</summary>
public sealed class BasicCopyHint : CopyHint
{
    /// <summary>Specific types to control via this hint.</summary>
    private static readonly HashSet<Type> _SupportedTypes = [typeof(string), typeof(object)];

    /// <inheritdoc/>
    protected internal sealed override (bool, object?) TryCopy(object source, DuplicatorChainer duplicator)
    {
        ArgumentGuard.ThrowIfNull(source, nameof(source));

        Type type = source.GetType();
        if (type.IsPrimitive
            || type.IsEnum
            || ValueRandom.ValueTypes.Contains(type)
            || _SupportedTypes.Contains(type)
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
