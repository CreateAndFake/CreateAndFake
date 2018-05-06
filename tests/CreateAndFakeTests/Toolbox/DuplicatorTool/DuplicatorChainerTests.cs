using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool
{
    /// <summary>Verifies behavior.</summary>
    public static class DuplicatorChainerTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void DuplicatorChainer_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<DuplicatorChainer>();
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void DuplicatorChainer_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<DuplicatorChainer>();
        }
    }
}
