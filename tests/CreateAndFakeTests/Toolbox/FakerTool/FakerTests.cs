using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
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
    }
}
