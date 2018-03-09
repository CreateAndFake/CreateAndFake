using System;
using System.Collections.Generic;
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
        public void New_NullCompareFuncInvalid()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new ValuerChainer(Tools.Randomizer.Create<ICollection<(int, int)>>(), null));
        }

        /// <summary>Verifies callback must be provided.</summary>
        [TestMethod]
        public void New_NullHashFuncInvalid()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new ValuerChainer(Tools.Randomizer.Create<ICollection<int>>(), null));
        }
    }
}
