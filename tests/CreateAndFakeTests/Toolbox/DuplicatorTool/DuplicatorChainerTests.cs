using System;
using System.Collections.Generic;
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
        public void New_NullFuncInvalid()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new DuplicatorChainer(Tools.Randomizer.Create<IDictionary<int, object>>(), null));
        }

        /// <summary>Verifies history is optional.</summary>
        [TestMethod]
        public void New_NullHistoryValid()
        {
            Tools.Asserter.IsNot(null, new DuplicatorChainer(null,
                Tools.Randomizer.Create<Func<object, IDictionary<int, object>, object>>()));
        }
    }
}
