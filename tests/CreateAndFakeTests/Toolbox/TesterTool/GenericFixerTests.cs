using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.TesterTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    public static class GenericFixerTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void GenericFixer_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<GenericFixer>();
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Theory, RandomData]
        public static void GenericFixer_NoParameterMutation(IRandom gen)
        {
            Tools.Tester.PreventsParameterMutation(new GenericFixer(gen, Tools.Randomizer));
        }
    }
}
