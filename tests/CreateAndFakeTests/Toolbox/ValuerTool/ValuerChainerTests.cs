using System;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.ValuerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class ValuerChainerTests
    {
        /// <summary>Verifies callback must be provided.</summary>
        [TestMethod]
        public void New_NullsInvalid()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new ValuerChainer(null, (o, c) => 0, (e, a, c) => null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new ValuerChainer(Tools.Valuer, (o, c) => 0, null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new ValuerChainer(Tools.Valuer, null, (e, a, c) => null));
        }
    }
}
