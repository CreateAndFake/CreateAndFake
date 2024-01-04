using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool;

/// <summary>Verifies behavior.</summary>
public static class DuplicatorChainerTests
{
    [Fact]
    internal static void DuplicatorChainer_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<DuplicatorChainer>();
    }

    [Fact]
    internal static void DuplicatorChainer_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<DuplicatorChainer>();
    }
}
