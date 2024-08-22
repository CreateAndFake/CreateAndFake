using System.Reflection;
using CreateAndFake.Toolbox;
using CreateAndFake.Toolbox.FakerTool;
using TypeExtensions = CreateAndFake.Toolbox.TypeExtensions;

namespace CreateAndFakeTests.Extensions;

/// <summary>Verifies behavior.</summary>
public static class TypeExtensionsTests
{
    [Fact]
    internal static void TypeExtensions_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException(typeof(TypeExtensions));
    }

    [Fact]
    internal static void Inherits_RaceConditionPrevented()
    {
        Type testType = Tools.Faker.Stub<object>(Assembly.GetExecutingAssembly()
            .GetTypes().Where(t => t.IsInterface).Where(t => t.IsVisible).ToArray()).GetType();

        Tools.Asserter.Is(true, Parallel.For(0, 10, i => testType.Inherits<object>()).IsCompleted);
    }

    [Theory, RandomData]
    internal static void FindLoadedTypes_IgnoresMissingAssembly(Fake<Assembly> assembly, FileNotFoundException error)
    {
        assembly.Setup(d => d.GetTypes(), Behavior.Throw(error));

        TypeExtensions.FindLoadedTypes(assembly.Dummy).Assert().IsEmpty();
    }

    [Theory, RandomData]
    internal static void FindLoadedTypes_IgnoresReflectError(Fake<Assembly> assembly, ReflectionTypeLoadException error)
    {
        assembly.Setup(d => d.GetTypes(), Behavior.Throw(error));

        TypeExtensions.FindLoadedTypes(assembly.Dummy).Assert().IsEmpty();
    }
}
