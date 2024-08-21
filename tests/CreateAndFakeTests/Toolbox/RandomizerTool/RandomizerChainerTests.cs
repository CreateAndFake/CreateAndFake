using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.RandomizerTool;

/// <summary>Verifies behavior.</summary>
public static class RandomizerChainerTests
{
    [Fact]
    internal static void RandomizerChainer_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<RandomizerChainer>();
    }

    [Fact]
    internal static void RandomizerChainer_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<RandomizerChainer>();
    }

    [Fact]
    internal static void Create_HandlesInfinites()
    {
        RandomizerChainer chainer = null;
        chainer = new RandomizerChainer(Tools.Faker, Tools.Gen, (t, c) => c.Create<ParentLoopSample>());

        Tools.Asserter.Throws<InfiniteLoopException>(() => chainer.Create(typeof(ChildWithParentSample), new ParentLoopSample()));
    }
}
