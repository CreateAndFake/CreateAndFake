using System;
using CreateAndFake;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandifferTool;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandifferTool
{
    /// <summary>Verifies behavior.</summary>
    public static class RandifferTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void Randiffer_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(Tools.Randiffer);
        }

        /// <summary>Verifies null defaults to empty.</summary>
        [Theory, RandomData]
        public static void Create_AcceptsNull(string value)
        {
            Tools.Asserter.IsNot(null, Tools.Randiffer.Branch<string>(null));
            Tools.Asserter.IsNot(value, Tools.Randiffer.Branch(value, null));
        }

        /// <summary>Verifies extra values to not equal work.</summary>
        [Theory, RandomData]
        public static void Create_ManyValuesWorks(string value1, string value2, string value3)
        {
            string result = Tools.Randiffer.Branch(value1, value2, value3);
            Tools.Asserter.IsNot(value1, result);
            Tools.Asserter.IsNot(value2, result);
            Tools.Asserter.IsNot(value3, result);
        }

        /// <summary>Verifies limiter limits attempts.</summary>
        [Theory, RandomData]
        public static void Create_TimesOut(Fake<IValuer> fakeValuer)
        {
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(true));

            Randiffer testInstance = new Randiffer(Tools.Randomizer, fakeValuer.Dummy, new Limiter(3));

            Tools.Asserter.Throws<TimeoutException>(
                () => testInstance.Branch(Tools.Randomizer.Create<DataSample>()));

            fakeValuer.VerifyTotalCalls(3);
        }

        /// <summary>Verifies create keeps trying until unequal instance is created.</summary>
        [Theory, RandomData]
        public static void Create_RepeatsUntilUnequal(Fake<IValuer> fakeValuer)
        {
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Series(true, true, true, false));

            Randiffer testInstance = new Randiffer(Tools.Randomizer, fakeValuer.Dummy, new Limiter(5));

            Tools.Asserter.IsNot(null, testInstance.Branch(Tools.Randomizer.Create<DataSample>()));
            fakeValuer.VerifyTotalCalls(4);
        }

        /// <summary>Verifies create keeps trying until unequal instance is created.</summary>
        [Theory, RandomData]
        public static void Create_RepeatsUntilBothUnequal(Fake<IValuer> fakeValuer)
        {
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Series(false, true, true, false, true, true, false, false));

            Randiffer testInstance = new Randiffer(Tools.Randomizer, fakeValuer.Dummy, new Limiter(5));

            Tools.Asserter.IsNot(null, testInstance.Branch(
                Tools.Randomizer.Create<DataSample>(),
                Tools.Randomizer.Create<DataSample>()));
            fakeValuer.VerifyTotalCalls(8);
        }
    }
}
