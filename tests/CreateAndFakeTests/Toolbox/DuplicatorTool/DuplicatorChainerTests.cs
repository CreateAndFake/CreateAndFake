using System;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class DuplicatorChainerTests
    {
        /// <summary>Verifies callback must be provided.</summary>
        [TestMethod]
        public void New_NullsInvalid()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new DuplicatorChainer(null, (o, c) => null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new DuplicatorChainer(Tools.Duplicator, null));
        }
    }
}
