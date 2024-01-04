using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.Toolbox;

/// <summary>Verifies issue is resolved.</summary>
public static class ToolSetTests
{
    [Fact]
    internal static void ToolSet_Creatable()
    {
        new ToolSet().Assert().IsNot(null);
        (ToolSet.CreateViaSeed(0) with { Gen = null }).Assert().IsNot(null);
    }
}
