using System;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.MutatorTool;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandifferTool
{
    /// <summary>Verifies behavior.</summary>
    public static class MutatorTests
    {
        [Fact]
        internal static void Mutator_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<Mutator>(Tools.Randomizer, Tools.Valuer, Limiter.Dozen);
        }

        [Fact]
        internal static void Mutator_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(Tools.Mutator);
        }

        [Theory, RandomData]
        internal static void Variant_AcceptsNull(string value)
        {
            Tools.Asserter.IsNot(null, Tools.Mutator.Variant<string>(null));
            Tools.Asserter.IsNot(value, Tools.Mutator.Variant(value, null));
        }

        [Theory, RandomData]
        internal static void Variant_ManyValuesWorks(int value)
        {
            int[] data = Enumerable.Repeat(0, 10000)
                .Select(v => Tools.Randomizer.Create<int>()).ToArray();

            int result = Tools.Mutator.Variant(value, data);
            Tools.Asserter.IsNot(value, result);
            Tools.Asserter.Is(false, data.Contains(result));
        }

        [Theory, RandomData]
        internal static void Variant_TimesOut(Fake<IValuer> fakeValuer)
        {
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(true));

            Mutator testInstance = new Mutator(Tools.Randomizer, fakeValuer.Dummy, new Limiter(3));

            Tools.Asserter.Throws<TimeoutException>(
                () => testInstance.Variant(Tools.Randomizer.Create<DataSample>()));

            fakeValuer.VerifyTotalCalls(3);
        }

        [Theory, RandomData]
        internal static void Variant_RepeatsUntilUnequal(Fake<IValuer> fakeValuer)
        {
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Series(true, true, true, false));

            Mutator testInstance = new Mutator(Tools.Randomizer, fakeValuer.Dummy, new Limiter(5));

            Tools.Asserter.IsNot(null, testInstance.Variant(Tools.Randomizer.Create<DataSample>()));
            fakeValuer.VerifyTotalCalls(4);
        }

        [Theory, RandomData]
        internal static void Variant_RepeatsUntilBothUnequal(Fake<IValuer> fakeValuer)
        {
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Series(false, true, true, false, true, true, false, false));

            Mutator testInstance = new Mutator(Tools.Randomizer, fakeValuer.Dummy, new Limiter(5));

            Tools.Asserter.IsNot(null, testInstance.Variant(
                Tools.Randomizer.Create<DataSample>(),
                Tools.Randomizer.Create<DataSample>()));
            fakeValuer.VerifyTotalCalls(8);
        }

        [Theory, RandomData]
        internal static void Modify_DataChanged(DataHolderSample data)
        {
            DataHolderSample dupe = Tools.Duplicator.Copy(data);

            Tools.Asserter.Is(true, Tools.Mutator.Modify(data));
            Tools.Asserter.ValuesNotEqual(dupe, data);
        }

        [Theory, RandomData]
        internal static void Modify_StatelessUnchanged(StatelessSample data)
        {
            Tools.Asserter.Is(false, Tools.Mutator.Modify(data));
        }
    }
}
