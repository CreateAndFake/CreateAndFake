using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.TesterTool;

namespace CreateAndFakeTests.Toolbox.TesterTool;

/// <summary>Verifies behavior.</summary>
public static class GenericFixerTests
{
    [Fact]
    internal static void GenericFixer_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<GenericFixer>();
    }

    [Theory, RandomData]
    internal static void GenericFixer_NoParameterMutation(IRandom gen)
    {
        Tools.Tester.PreventsParameterMutation(new GenericFixer(gen, Tools.Randomizer));
    }
}
