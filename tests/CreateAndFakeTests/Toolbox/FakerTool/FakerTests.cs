using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.FakerTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool;

/// <summary>Verifies behavior.</summary>
public static class FakerTests
{
    [Fact]
    internal static void Faker_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<Faker>();
    }

    [Fact]
    internal static void Faker_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<Faker>();
    }

    [Fact]
    internal static void New_NullValuerValid()
    {
        Tools.Asserter.IsNot(null, new Faker(null));
    }

    [Fact]
    internal static void Mock_SampleWorks()
    {
        Fake<DataHolderSample> sample = new Faker(Tools.Valuer).Mock<DataHolderSample>();
        Tools.Asserter.Is(true, sample.ThrowByDefault);
        Tools.Asserter.Throws<FakeCallException>(() => sample.Dummy.HasNested(sample.Dummy.NestedValue));
    }

    [Fact]
    internal static void Stub_SampleWorks()
    {
        Fake<DataHolderSample> sample = new Faker(Tools.Valuer).Stub<DataHolderSample>();
        Tools.Asserter.Is(false, sample.ThrowByDefault);
        Tools.Asserter.Is(false, sample.Dummy.HasNested(null));
    }

    [Fact]
    internal static void Supports_InvalidTypesFalse()
    {
        Tools.Asserter.Is(false, Tools.Faker.Supports<int>());
        Tools.Asserter.Is(false, Tools.Faker.Supports<Array>());
        Tools.Asserter.Is(false, Tools.Faker.Supports<InternalSample>());
        Tools.Asserter.Is(false, Tools.Faker.Supports(typeof(void*)));
        Tools.Asserter.Is(false, Tools.Faker.Supports(typeof(ConstraintSample<,>)));
    }

    [Fact]
    internal static void Inject_HandlesValues()
    {
        Injected<FakeHolderSample> sample = Tools.Faker.InjectMocks<FakeHolderSample>();
        Tools.Asserter.Is(0, sample.Dummy.Value1);
        Tools.Asserter.Is(null, sample.Dummy.Value2);
    }

    [Fact]
    internal static void Inject_ConstructorRequired()
    {
        Tools.Asserter.Throws<InvalidOperationException>(() => Tools.Faker.InjectStubs<IOnlyMockSample>());
    }

    [Theory, RandomData]
    internal static void Inject_UsesValues(int num, string text)
    {
        Injected<FakeHolderSample> sample = Tools.Faker.InjectMocks<FakeHolderSample>(
            null, Tools.Faker.Stub<AbstractFakeSample>(), num, text);

        Tools.Asserter.Is(null, sample.Dummy.Sample1.Text);
        Tools.Asserter.Throws<FakeCallException>(() => sample.Dummy.Sample2.Calc());
        Tools.Asserter.Is(num, sample.Dummy.Value1);
        Tools.Asserter.Is(text, sample.Dummy.Value2);
    }

    [Fact]
    internal static void InjectMocks_AreMocks()
    {
        Injected<FakeHolderSample> sample = Tools.Faker.InjectMocks<FakeHolderSample>();

        Tools.Asserter.Throws<FakeCallException>(() => sample.Dummy.Sample1.Calc());
        Tools.Asserter.Throws<FakeCallException>(() => sample.Dummy.Sample2.Text);
    }

    [Fact]
    internal static void InjectStubs_AreStubs()
    {
        Injected<FakeHolderSample> sample = Tools.Faker.InjectStubs<FakeHolderSample>();

        Tools.Asserter.Is(0, sample.Dummy.Sample1.Calc());
        Tools.Asserter.Is(null, sample.Dummy.Sample2.Text);
    }
}
