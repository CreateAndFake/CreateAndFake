using System;
using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.AsserterTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class AssertExceptionTests : ExceptionTestBase<AssertException>
    {
        /// <summary>Verifies a null message defaults.</summary>
        [TestMethod]
        public void AssertException_UnknownMessage()
        {
            Tools.Asserter.Is("Unknown assert failure.", new AssertException(null, null).Message);
        }

        /// <summary>Verifies included data.</summary>
        [TestMethod]
        public void AssertException_MessageFormat()
        {
            string message = Tools.Randomizer.Create<string>();
            string details = Tools.Randomizer.Create<string>();
            string content = Tools.Randomizer.Create<string>();
            string nl = Environment.NewLine;

            Tools.Asserter.Is(message, new AssertException(message, null).Message);

            Tools.Asserter.Is(message + nl + "Details: " + details,
                new AssertException(message, details).Message);

            Tools.Asserter.Is(message + nl + "Content:" + nl + content,
                new AssertException(message, null, content).Message);

            Tools.Asserter.Is(message + nl + "Details: " + details + nl + "Content:" + nl + content,
                new AssertException(message, details, content).Message);
        }
    }
}
