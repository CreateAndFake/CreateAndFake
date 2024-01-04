using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool;

/// <summary>Verifies behavior.</summary>
public static class ValuerChainerTests
{
    [Fact]
    internal static void ValuerChainer_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<ValuerChainer>();
    }

    [Fact]
    internal static void ValuerChainer_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<ValuerChainer>();
    }
}
