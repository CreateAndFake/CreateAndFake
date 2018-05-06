﻿using CreateAndFake;
using CreateAndFake.Toolbox.RandomizerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class RandomizerChainerTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void RandomizerChainer_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<RandomizerChainer>();
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void RandomizerChainer_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<RandomizerChainer>();
        }
    }
}
