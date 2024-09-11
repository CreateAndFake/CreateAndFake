using CreateAndFake.Design;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.MutatorTool;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.MutatorTool;

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
        Tools.Mutator.Variant<string>(null).Assert().IsNot(null);
        Tools.Mutator.Variant(value, null).Assert().IsNot(value).And.IsNot(null);
    }

    [Theory, RandomData]
    internal static void Variant_ManyValuesWorks(int value, [Size(10000)] int[] data)
    {
        int result = Tools.Mutator.Variant(value, data);
        result.Assert().IsNot(value).Also(data).ContainsNot(result);
    }

    [Theory, RandomData]
    internal static void Variant_TimesOut([Fake] IValuer fakeValuer, DataSample sample)
    {
        fakeValuer.Equals(Arg.Any<object>(), Arg.Any<object>()).SetupReturn(true);

        new Mutator(Tools.Randomizer, fakeValuer, new Limiter(3))
            .Assert(t => t.Variant(sample))
            .Throws<TimeoutException>();

        fakeValuer.VerifyAllCalls(Times.Exactly(3));
    }

    [Theory, RandomData]
    internal static void Variant_RepeatsUntilUnequal([Fake] IValuer fakeValuer, DataSample sample)
    {
        fakeValuer.Equals(Arg.Any<object>(), Arg.Any<object>()).SetupCall(Behavior.Series(true, true, true, false));

        new Mutator(Tools.Randomizer, fakeValuer, new Limiter(5))
            .Variant(sample)
            .Assert()
            .IsNot(null);

        fakeValuer.VerifyAllCalls(Times.Exactly(4));
    }

    [Theory, RandomData]
    internal static void Variant_RepeatsUntilBothUnequal(
        [Fake] IValuer fakeValuer, DataSample sample1, DataSample sample2)
    {
        fakeValuer.Equals(Arg.Any<object>(), Arg.Any<object>()).SetupCall(
            Behavior.Series(false, true, true, false, true, true, false, false));

        new Mutator(Tools.Randomizer, fakeValuer, new Limiter(5))
            .Variant(sample1, sample2)
            .Assert()
            .IsNot(null);

        fakeValuer.VerifyAllCalls(8);
    }

    [Theory, RandomData]
    internal static void Modify_DataChanged(DataHolderSample data)
    {
        DataHolderSample dupe = data.CreateDeepClone();

        Tools.Mutator.Modify(data).Assert().Is(true);

        data.Assert().IsNot(dupe);
    }

    [Theory, RandomData]
    internal static void Modify_StatelessUnchanged(StatelessSample data)
    {
        Tools.Mutator.Modify(data).Assert().Is(false);
    }
}
