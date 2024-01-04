﻿using System;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

#pragma warning disable SYSLIB0050 // 'Type.IsSerializable' is obsolete

/// <summary>Handles generation of exceptions.</summary>
public sealed class ExceptionCreateHint : CreateHint
{
    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (!type.Inherits<Exception>())
        {
            return (false, null);
        }

        ConstructorInfo[] options = type.FindLocalSubclasses()
            .Where(t => t.IsVisible)
            .Where(t => t.IsSerializable)
#if NETSTANDARD // Security exceptions don't work with default serialization in .NET full.
            .Where(t => !t.Namespace.StartsWith("System.Security", StringComparison.Ordinal))
#endif
            .Select(t => t.GetConstructor([typeof(string)]))
            .Where(c => c != null)
            .ToArray();

        return (options.Length != 0)
            ? (true, randomizer.Gen.NextItem(options).Invoke([randomizer.Create<string>()]))
            : (false, null);
    }
}

#pragma warning restore SYSLIB0050 // 'Type.IsSerializable' is obsolete
