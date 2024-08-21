using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue002Tests
{
    [Fact]
    internal static void Issue002_CallDataEncapsulated()
    {
        typeof(CallData).IsPublic.Assert().Is(false);
    }
}
