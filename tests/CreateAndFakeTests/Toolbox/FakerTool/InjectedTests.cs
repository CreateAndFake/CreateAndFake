using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.FakerTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class InjectedTests
    {
        [Theory, RandomData]
        internal static void Injected_GuardsNulls(Injected<InjectSample> sample)
        {
            Tools.Tester.PreventsNullRefException(sample, sample.Fakes);
        }

        [Theory, RandomData]
        internal static void Injected_NoParameterMutation(Injected<InjectSample> sample)
        {
            Tools.Tester.PreventsParameterMutation(sample, sample.Fakes);
        }

        [Theory, RandomData]
        internal static void Fake_CanFindByDummy(Injected<FakeHolderSample> sample)
        {
            Tools.Asserter.Is(sample.Fake<IFakeSample>(), sample.Fake(sample.Dummy.Sample1));
            Tools.Asserter.Is(sample.Fake<AbstractFakeSample>(), sample.Fake(sample.Dummy.Sample2));
        }
    }
}
