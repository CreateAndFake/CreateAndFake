using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue002Tests
    {
        [Fact]
        internal static void Issue_CallDataEncapsulated()
        {
            typeof(CallData).IsPublic.Assert().Is(false);
        }
    }
}
