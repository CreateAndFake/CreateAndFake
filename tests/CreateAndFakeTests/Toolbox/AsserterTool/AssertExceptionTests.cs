using System;
using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool
{
    /// <summary>Verifies behavior.</summary>
    public sealed class AssertExceptionTests : ExceptionTestBase<AssertException>
    {
        [Fact]
        internal static void AssertException_UnknownMessageDefault()
        {
            Tools.Asserter.Is("Unknown assert failure.", new AssertException(null, null).Message);
        }

        [Theory, RandomData]
        internal static void AssertException_MessageFormat(string message, string details, string content)
        {
            string nl = Environment.NewLine;

            Tools.Asserter.Is(message, new AssertException(message, null).Message);

            Tools.Asserter.Is(message + nl + "Details: " + details,
                new AssertException(message, details).Message);

            Tools.Asserter.Is(message + nl + "Content: " + nl + content,
                new AssertException(message, null, content).Message);

            Tools.Asserter.Is(message + nl + "Details: " + details + nl + "Content: " + nl + content,
                new AssertException(message, details, content).Message);
        }
    }
}
