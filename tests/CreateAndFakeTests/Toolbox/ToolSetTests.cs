namespace CreateAndFakeTests.Toolbox;

/// <summary>Verifies issue is resolved.</summary>
public static class ToolSetTests
{
    [Fact]
    internal static void ToolSet_Creatable()
    {
        ToolSet.CreateViaSeed(0).Assert().IsNot(null);
    }
}
