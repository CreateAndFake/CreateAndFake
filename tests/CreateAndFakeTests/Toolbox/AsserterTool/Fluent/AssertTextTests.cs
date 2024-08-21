using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool.Fluent;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

/// <summary>Verifies behavior.</summary>
public static class AssertTextTests
{
    [Fact]
    internal static void AssertText_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertText>();
    }

    [Fact]
    internal static void AssertText_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertText>();
    }
}
