using System;
using CreateAndFake;
using CreateAndFake.Design;
using Xunit;

namespace CreateAndFakeTests.Design;

/// <summary>Verifies behavior.</summary>
public static class ArgumentGuardTests
{
    [Theory, RandomData]
    internal static void ThrowIfNull_NoExceptionWithNonNull(object value, string name)
    {
        ArgumentGuard.ThrowIfNull(value, name);
    }

    [Theory, RandomData]
    internal static void ThrowIfNull_ExceptionWithNull(string name)
    {
        Tools.Asserter.Throws<ArgumentNullException>(() => ArgumentGuard.ThrowIfNull(null, name));
    }
}
