using CreateAndFake;
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
        [Fact]
        public static void GenericFixer_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<GenericFixer>();
        }
    }
}
