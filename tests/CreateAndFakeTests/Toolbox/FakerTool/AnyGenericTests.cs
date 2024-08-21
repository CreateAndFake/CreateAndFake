using System.Reflection;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.Toolbox.FakerTool;

/// <summary>Verifies behavior.</summary>
public static class AnyGenericTests
{
    [Fact]
    internal static void VoidType_PrivateConstructor()
    {
        ConstructorInfo constructor = typeof(AnyGeneric).GetConstructors(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Single();
        Tools.Asserter.Is(true, constructor.IsPrivate);
        constructor.Invoke([]);
    }
}
