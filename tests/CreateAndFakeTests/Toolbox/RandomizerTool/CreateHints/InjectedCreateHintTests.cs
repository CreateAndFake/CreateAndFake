using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.FakerTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class InjectedCreateHintTests : CreateHintTestBase<InjectedCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly InjectedCreateHint _TestInstance = new InjectedCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[]
        {
            typeof(Injected<FakeHolderSample>),
            typeof(Injected<InjectSample>),
            typeof(Injected<InjectMockSample>),
            typeof(Injected<MismatchDataSample>),
            typeof(Injected<StructSample>)
        };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[]
        {
            typeof(object),
            typeof(IUnimplementedSample)
        };

        /// <summary>Sets up the tests.</summary>
        public InjectedCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

        [Theory, RandomData]
        internal void Create_ValidInjections(Injected<InjectMockSample> sample)
        {
            Tools.Asserter.IsNot(null, sample.Fake<IOnlyMockSample>());
            Tools.Asserter.IsNot(null, sample.Fake<IOnlyMockSample>(1));

            Tools.Asserter.IsNot(sample.Fake<IOnlyMockSample>(), sample.Fake<IOnlyMockSample>(1));

            sample.Fake<IOnlyMockSample>().Setup(m => m.FailIfNotMocked(), Behavior.Returns(false, Times.Once));
            sample.Dummy.TestIfMockedSeparately();
            sample.Fake<IOnlyMockSample>().VerifyAll(Times.Once);
            sample.VerifyAll();
        }

        [Fact]
        internal void Create_NoConstructorThrows()
        {
            Tools.Asserter.Throws<InvalidOperationException>(
                () => Tools.Randomizer.Create<Injected<IUnimplementedSample>>());
        }

        [Theory, RandomData]
        internal void Create_ValuesRandom(Injected<FakeHolderSample> sample)
        {
            Tools.Asserter.IsNot(0, sample.Dummy.Value1);
            Tools.Asserter.IsNot(null, sample.Dummy.Value2);
        }
    }
}
