using System;
using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.RandomizerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class RandomizerChainerTests
    {
        /// <summary>Verifies bad nulls are prevented.</summary>
        [TestMethod]
        public void New_InvalidNullsThrow()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new RandomizerChainer(Tools.Faker, null, (t, c) => null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new RandomizerChainer(Tools.Faker, new FastRandom(), null));
        }
    }
}
