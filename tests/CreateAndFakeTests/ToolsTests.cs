using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CreateAndFake;
using CreateAndFake.Design.Content;
using CreateAndFake.Design.Context;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox;
using CreateAndFake.Toolbox.AsserterTool.Fluent;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests;

/// <summary>Verifies behavior.</summary>
public static class ToolsTests
{
    /// <summary>Flags representing mutable data.</summary>
    private const BindingFlags _Mutable = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    [Fact]
    internal static void Tools_IntegrationWorks()
    {
        DataHolderSample original = Tools.Randomizer.Create<DataHolderSample>();
        DataHolderSample variant = Tools.Mutator.Variant(original);
        DataHolderSample dupe = Tools.Duplicator.Copy(original);

        Tools.Asserter.ValuesEqual(original, dupe);
        Tools.Asserter.ValuesNotEqual(original, variant);

        Fake<DataHolderSample> faked = Tools.Faker.Mock<DataHolderSample>();
        faked.Setup(
            m => m.HasNested(dupe),
            Behavior.Returns(true, Times.Once));

        Tools.Asserter.Is(true, faked.Dummy.HasNested(original),
            "Value equality did not work for args.");

        faked.VerifyAll(Times.Once);
    }

    [Theory, RandomData]
    internal static void Tools_HandlesInfinites(InfiniteSample sample)
    {
        Tools.Asserter.Throws<TimeoutException>(
            () => Tools.Mutator.Variant(sample));

        InfiniteSample dupe = Tools.Duplicator.Copy(sample);

        Tools.Asserter.Is(sample, dupe);
        Tools.Asserter.Is(Tools.Valuer.GetHashCode(sample), Tools.Valuer.GetHashCode(dupe));
    }

    [Fact]
    internal static void Tools_AllCreateAndFakeTypesWork()
    {
        Type[] ignore = [
            typeof(Arg),
            typeof(Fake),
            typeof(Fake<>),
            typeof(VoidType),
            typeof(AnyGeneric),
            typeof(Injected<>),
            typeof(DuplicatorChainer),
            typeof(ValuerChainer),
            typeof(Emitter),
            typeof(Behavior),
            typeof(Behavior<>),
            typeof(AssertObjectBase<>),
            typeof(AssertGroupBase<>),
            typeof(AssertTextBase<>),
            typeof(AssertBehaviorBase<>),
            typeof(AssertComparableBase<>),
            typeof(DataRandom),
            typeof(BaseDataContext),
            typeof(PersonContext),
            typeof(ToolSet),
            typeof(Tools),
            typeof(FakeAttribute),
            typeof(StubAttribute)
        ];

        foreach (Type type in typeof(Tools).Assembly.GetTypes()
            .Where(t => !(t.IsAbstract && t.IsSealed))
            .Where(t => !ignore.Contains(t))
            .Where(t => !t.IsNestedPrivate)
            .Where(t => t.GetCustomAttribute<CompilerGeneratedAttribute>() == null))
        {
            try
            {
                TestTrip(type);
            }
            catch (Exception e)
            {
                Tools.Asserter.Fail(e, $"Failed testing type '{type.Name}'.");
                throw;
            }
        }
    }

    [Fact]
    internal static void Tools_ExceptionTypesWork()
    {
        Type type = typeof(Exception);

        for (int i = 0; i < 100; i++)
        {
            TestTrip(type);
        }
    }

    /// <summary>Verifies the type works with the tools.</summary>
    /// <param name="type">Type to test.</param>
    private static void TestTrip(Type type)
    {
        string failMessage = "Behavior did not work for type '" + type.FullName + "'.";
        object
            original = null,
            variant = null,
            dupe = null;
        try
        {
            original = Tools.Randomizer.Create(type);
            dupe = Tools.Duplicator.Copy(original);
            Tools.Asserter.ValuesEqual(original, dupe, failMessage);
            Tools.Asserter.ValuesEqual(
                Tools.Valuer.GetHashCode(original),
                Tools.Valuer.GetHashCode(dupe), $"HashCode {failMessage}");

            if (type.GetProperties(_Mutable).Length != 0 || type.GetFields(_Mutable).Length != 0)
            {
                variant = Tools.Mutator.Variant(type, original);
                Tools.Asserter.ValuesNotEqual(original, variant, failMessage);
                Tools.Asserter.ValuesNotEqual(
                    Tools.Valuer.GetHashCode(original),
                    Tools.Valuer.GetHashCode(variant), failMessage);

                if (Tools.Mutator.Modify(original))
                {
                    Tools.Asserter.ValuesNotEqual(dupe, original);
                }
            }

            if (Tools.Faker.Supports(type) && !type.Inherits<IDisposable>())
            {
                Tools.Faker.Mock(type);
            }
        }
        finally
        {
            Disposer.Cleanup(original, variant, dupe);
        }
    }
}
