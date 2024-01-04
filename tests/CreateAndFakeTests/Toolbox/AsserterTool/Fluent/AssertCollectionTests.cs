using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool.Fluent;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

/// <summary>Verifies behavior.</summary>
public static class AssertCollectionTests
{
    [Fact]
    internal static void AssertCollection_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertGroup>();
    }

    [Fact]
    internal static void AssertCollection_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertGroup>();
    }
}
