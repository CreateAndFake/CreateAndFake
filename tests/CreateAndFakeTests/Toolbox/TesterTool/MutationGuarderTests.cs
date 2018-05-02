using System;
using CreateAndFake;
using CreateAndFake.Toolbox.TesterTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    public static class MutationGuarderTests
    {
        /// <summary>Instance to test with.</summary>
        private static MutationGuarder s_TestInstance = new MutationGuarder(
            new GenericFixer(Tools.Gen, Tools.Randomizer), Tools.Randomizer,
            Tools.Duplicator, Tools.Asserter, new TimeSpan(0, 0, 0, 0, 100));

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void MutationGuarder_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(s_TestInstance);
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void MutationGuarder_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(s_TestInstance);
        }
    }
}
