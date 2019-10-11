using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool.Proxy
{
    /// <summary>Verifies behavior.</summary>
    public sealed class FakeCallExceptionTests : ExceptionTestBase<FakeCallException>
    {
        [Fact]
        internal static void FakeCallException_MultiNullsOkay()
        {
            Tools.Asserter.IsNot(null, new FakeCallException(null, null));
        }
    }
}
