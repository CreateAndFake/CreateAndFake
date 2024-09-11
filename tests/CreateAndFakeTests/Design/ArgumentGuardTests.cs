using CreateAndFake.Design;

namespace CreateAndFakeTests.Design;

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
        name.Assert(n => ArgumentGuard.ThrowIfNull(null, n)).Throws<ArgumentNullException>();
    }
}
