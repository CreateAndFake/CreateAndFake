using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool;

/// <summary>Verifies behavior.</summary>
public static class DifferenceTests
{
    [Fact]
    internal static void Difference_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<Difference>();
    }

    [Fact]
    internal static void Difference_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<Difference>();
    }
}
