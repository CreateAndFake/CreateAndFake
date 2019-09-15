using CreateAndFake;
using CreateAndFake.Toolbox.RandomizerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
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
    }
}
