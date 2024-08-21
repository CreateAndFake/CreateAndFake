using System.Reflection;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.Toolbox.FakerTool;

/// <summary>Verifies behavior.</summary>
public static class VoidTypeTests
{
    [Fact]
    internal static void VoidType_PrivateConstructor()
    {
        ConstructorInfo constructor = typeof(VoidType).GetConstructors(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Single();
        Tools.Asserter.Is(true, constructor.IsPrivate);
        constructor.Invoke([]);
    }
}
