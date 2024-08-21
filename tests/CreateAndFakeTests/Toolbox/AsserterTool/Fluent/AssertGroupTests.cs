using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

/// <summary>Verifies behavior.</summary>
public static class AssertCollectionTests
{
    [Fact]
    internal static void AssertGroup_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertGroup>();
    }

    [Fact]
    internal static void AssertGroup_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertGroup>();
    }
}
