using System;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.ValuerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class DifferenceTests
    {
        /// <summary>Verifies acceptable null arguments work.</summary>
        [TestMethod]
        public void New_ValidNullsWork()
        {
            Tools.Asserter.IsNot(null, new Difference((Type)null, null));
            Tools.Asserter.IsNot(null, new Difference((object)null, null));
        }

        /// <summary>Verifies unacceptable null arguments throw.</summary>
        [TestMethod]
        public void New_InvalidNullsThrow()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Difference(Tools.Randomizer.Create<int>(), null));

            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Difference((MemberInfo)null, Tools.Randomizer.Create<Difference>()));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Difference(Tools.Randomizer.Create<MemberInfo>(), null));

            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Difference((string)null, Tools.Randomizer.Create<Difference>()));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Difference(Tools.Randomizer.Create<string>(), null));
        }
    }
}
