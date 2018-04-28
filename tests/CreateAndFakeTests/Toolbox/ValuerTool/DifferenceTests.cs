using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class DifferenceTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void Difference_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<Difference>();
        }
    }
}
