using System.Reflection;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.FakerTool.TestSamples;

namespace CreateAndFakeTests.Toolbox.FakerTool.Proxy;

/// <summary>Verifies behavior.</summary>
public static class SubclasserTests
{
    [Fact]
    internal static void Subclasser_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException(typeof(Subclasser));
    }

    [Fact]
    internal static void Subclasser_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation(typeof(Subclasser));
    }

    [Fact]
    internal static void Create_InterfacesWork()
    {
        Tools.Asserter.IsNot(null, Subclasser.Create<IFakeSample>());
        Tools.Asserter.IsNot(null, Subclasser.Create<IFakeSample>(typeof(IClashingFakeSample)));
    }

    [Fact]
    internal static void Create_ClassesWork()
    {
        Tools.Asserter.IsNot(null, Subclasser.Create<AbstractFakeSample>());
        Tools.Asserter.IsNot(null, Subclasser.Create<VirtualFakeSample>());
    }

    [Fact]
    internal static void Create_BothWork()
    {
        Tools.Asserter.IsNot(null, Subclasser.Create<AbstractFakeSample>(typeof(IFakeSample)));
        Tools.Asserter.IsNot(null, Subclasser.Create<VirtualFakeSample>(
            typeof(IFakeSample), typeof(IClashingFakeSample)));
    }

    [Fact]
    internal static void Create_IFakedDefault()
    {
        Tools.Asserter.Is(true, Subclasser.Create<object>() is IFaked);
        Tools.Asserter.IsNot(null, Subclasser.Create(null, null));
    }

    [Fact]
    internal static void Create_IFakedFunctional()
    {
        Tools.Asserter.IsNot(null, Subclasser.Create<IFaked>().FakeMeta);
    }

    [Fact]
    internal static void Create_OnlyMultipleInterfaces()
    {
        Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create<DataSample>(typeof(object)));
    }

    [Fact]
    internal static void Create_SealedTypesThrow()
    {
        Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create<string>());
    }

    [Fact]
    internal static void CreateInfo_NoDuplicatesCreated()
    {
        Tools.Asserter.Is(Subclasser.CreateInfo(typeof(IFakeSample), typeof(IClashingFakeSample)),
            Subclasser.CreateInfo(typeof(IClashingFakeSample), typeof(IFakeSample)));
    }

    [Fact]
    internal static void CreateInfo_IgnoreDupeInterfaces()
    {
        Tools.Asserter.Is(Subclasser.CreateInfo(typeof(IFakeSample)),
            Subclasser.CreateInfo(typeof(IFakeSample), typeof(IFakeSample)));
    }

    [Fact]
    internal static void Create_DefinedGenericsWork()
    {
        Tools.Asserter.IsNot(null, Subclasser.Create<ConstraintSample<int, DataSample>>());
        Tools.Asserter.IsNot(null, Subclasser.Create<ConstraintSample<bool, DataSample>>());
    }

    [Fact]
    internal static void Create_UndefinedGenericsThrow()
    {
        Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create(typeof(ConstraintSample<,>)));
    }

    [Fact]
    internal static void Create_PointersThrow()
    {
        Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create(typeof(void*)));
    }

    [Fact]
    internal static void Create_InternalTypesThrow()
    {
        Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create<InternalSample>());
    }

    [Fact]
    internal static void Supports_FalseWithWithNonVisibleTypes()
    {
        TypeAttributes invisibleAttributes
            = TypeAttributes.NotPublic
            | TypeAttributes.Class;

        Type type = Tools.Faker.Stub<Type>().Dummy;

        type.ToFake().Setup("GetAttributeFlagsImpl", [], Behavior.Returns(invisibleAttributes));
        type.ToFake().Setup("HasElementTypeImpl", [], Behavior.Returns(false));
        type.ToFake().Setup("IsPointerImpl", [], Behavior.Returns(false));
        type.Assembly.SetupReturn(typeof(object).Assembly);
        type.Name.SetupReturn("TestInvisibleType");

        Tools.Asserter
            .Throws<ArgumentException>(() => Subclasser.Create(type))
            .Message
            .Assert()
            .Contains("InternalsVisibleTo");
    }
}
