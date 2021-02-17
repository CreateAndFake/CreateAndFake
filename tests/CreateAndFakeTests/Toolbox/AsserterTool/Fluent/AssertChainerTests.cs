using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool.Fluent;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent
{
    /// <summary>Verifies behavior.</summary>
    public static class AssertChainerTests
    {
        [Fact]
        internal static void AssertChainer_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<AssertChainer<object>>();
        }

        [Fact]
        internal static void AssertChainer_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<AssertChainer<object>>();
        }

        [Theory, RandomData]
        internal static void And_PassesBackInstance(object data)
        {
            Tools.Asserter.Is(data, new AssertChainer<object>(data, Tools.Gen, Tools.Valuer).And);
        }
    }
}
