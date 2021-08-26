using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.Extensions
{
    /// <summary>Verifies behavior.</summary>
    public static class CreateExtensionsTests
    {
        [Fact]
        internal static void CloneExtensions_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(typeof(CreateExtensions));
        }

        [Fact]
        internal static void CloneExtensions_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(typeof(CreateExtensions));
        }
    }
}