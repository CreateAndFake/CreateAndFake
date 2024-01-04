using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool.Fluent;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

/// <summary>Verifies behavior.</summary>
public static class AssertObjectTests
{
    [Fact]
    internal static void AssertObject_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertObject>();
    }

    [Fact]
    internal static void AssertObject_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertObject>();
    }
}
