using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool.Fluent;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

/// <summary>Verifies behavior.</summary>
public static class AssertBehaviorTests
{
    [Fact]
    internal static void AssertBehavior_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertBehavior>();
    }

    [Fact]
    internal static void AssertBehavior_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertBehavior>();
    }
}
