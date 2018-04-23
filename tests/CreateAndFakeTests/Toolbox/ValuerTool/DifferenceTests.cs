using System;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class DifferenceTests
    {
        /// <summary>Verifies acceptable null arguments work.</summary>
        [Fact]
        public static void New_ValidNullsWork()
        {
            Tools.Asserter.IsNot(null, new Difference((Type)null, null));
            Tools.Asserter.IsNot(null, new Difference((object)null, null));
        }

        /// <summary>Verifies unacceptable null arguments throw.</summary>
        [Fact]
        public static void New_InvalidNullsThrow()
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
