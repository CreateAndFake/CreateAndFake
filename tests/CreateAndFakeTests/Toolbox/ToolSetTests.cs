namespace CreateAndFakeTests.Toolbox;

public static class ToolSetTests
{
    [Fact]
    internal static void ToolSet_Creatable()
    {
        ToolSet.CreateViaSeed(0).Assert().IsNot(null);
    }
}
