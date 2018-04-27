using System;
using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.RandomizerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class RandomizerChainerTests
    {
        /// <summary>Verifies bad nulls are prevented.</summary>
        [Fact]
        public static void New_InvalidNullsThrow()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new RandomizerChainer(Tools.Faker, null, (t, c) => null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new RandomizerChainer(Tools.Faker, new FastRandom(), null));
        }
    }
}
