﻿using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.FakerTool.TestSamples;

namespace CreateAndFakeTests.Toolbox.FakerTool;

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
        new Faker(null).Assert().Pass();
    }

    [Fact]
    internal static void Mock_SampleWorks()
    {
        Fake<DataHolderSample> sample = Tools.Faker.Mock<DataHolderSample>();
        sample.ThrowByDefault.Assert().Is(true);
        sample.Dummy.Assert(d => d.HasNested(sample.Dummy.NestedValue)).Throws<FakeCallException>();
    }

    [Fact]
    internal static void Stub_SampleWorks()
    {
        Fake<DataHolderSample> sample = Tools.Faker.Stub<DataHolderSample>();
        sample.ThrowByDefault.Assert().Is(false);
        sample.Dummy.HasNested(null).Assert().Is(false);
    }

    [Fact]
    internal static void Supports_InvalidTypesFalse()
    {
        Tools.Faker.Supports<int>().Assert().Is(false);
        Tools.Faker.Supports<Array>().Assert().Is(false);
        Tools.Faker.Supports<InternalSample>().Assert().Is(false);
        Tools.Faker.Supports(typeof(void*)).Assert().Is(false);
        Tools.Faker.Supports(typeof(ConstraintSample<,>)).Assert().Is(false);
    }

    [Fact]
    internal static void Inject_HandlesValues()
    {
        Injected<FakeHolderSample> sample = Tools.Faker.InjectMocks<FakeHolderSample>();
        sample.Dummy.Value1.Assert().Is(0);
        sample.Dummy.Value2.Assert().Is(null);
    }

    [Fact]
    internal static void Inject_ConstructorRequired()
    {
        Tools.Faker.Assert(f => f.InjectStubs<IOnlyMockSample>()).Throws<InvalidOperationException>();
    }

    [Theory, RandomData]
    internal static void Inject_UsesValues(int num, string text)
    {
        Injected<FakeHolderSample> sample = Tools.Faker.InjectMocks<FakeHolderSample>(
            null, Tools.Faker.Stub<AbstractFakeSample>(), num, text);

        sample.Dummy.Sample1.Text.Assert().Is(null);
        sample.Dummy.Sample2.Assert(s => s.Calc()).Throws<FakeCallException>();
        sample.Dummy.Value1.Assert().Is(num);
        sample.Dummy.Value2.Assert().Is(text);
    }

    [Fact]
    internal static void InjectMocks_AreMocks()
    {
        Injected<FakeHolderSample> sample = Tools.Faker.InjectMocks<FakeHolderSample>();

        sample.Dummy.Sample1.Assert(s => s.Calc()).Throws<FakeCallException>();
        sample.Dummy.Sample2.Assert(s => s.Text).Throws<FakeCallException>();
    }

    [Fact]
    internal static void InjectStubs_AreStubs()
    {
        Injected<FakeHolderSample> sample = Tools.Faker.InjectStubs<FakeHolderSample>();

        sample.Dummy.Sample1.Calc().Assert().Is(0);
        sample.Dummy.Sample2.Text.Assert().Is(null);
    }
}
