﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using CreateAndFake;
using CreateAndFake.Design;
using Xunit;

namespace CreateAndFakeTests.TestBases;

/// <summary>Handles testing exceptions.</summary>
/// <typeparam name="T">Exception type to test.</typeparam>
public abstract class ExceptionTestBase<T> where T : Exception
{
    /// <summary>Verifies the default constructor is present for serialization but private.</summary>
    [Fact]
    public void Exception_DefaultConstructorPrivate()
    {
        Tools.Asserter.Is(null, typeof(T).GetConstructor(Type.EmptyTypes));
        Tools.Asserter.IsNot(null, Activator.CreateInstance(typeof(T), true));
    }

    /// <summary>Verifies that the exception can make an xml roundtrip.</summary>
    [Theory, RandomData]
    public void Exception_XmlSerializes(T original)
    {
        ArgumentGuard.ThrowIfNull(original, nameof(original));

        DataContractSerializer formatter = new(typeof(T),
            new[] { original.InnerException }.Where(t => t != null).Select(t => t.GetType()));

        using (MemoryStream stream = new())
        {
            formatter.WriteObject(stream, original);
            _ = stream.Seek(0, SeekOrigin.Begin);

            Tools.Asserter.Is(original, formatter.ReadObject(stream));
        }
    }

    /// <summary>Verifies that the exception can make a json roundtrip.</summary>
    [Fact]
    public void Exception_JsonSerializes()
    {
        T original;
        do
        {
            original = Tools.Randomizer.Create<T>();
        } while (original.InnerException != null);

        DataContractJsonSerializer formatter = new(typeof(T));

        using (MemoryStream stream = new())
        {
            formatter.WriteObject(stream, original);
            _ = stream.Seek(0, SeekOrigin.Begin);

            Tools.Asserter.Is(original, formatter.ReadObject(stream));
        }
    }
}
