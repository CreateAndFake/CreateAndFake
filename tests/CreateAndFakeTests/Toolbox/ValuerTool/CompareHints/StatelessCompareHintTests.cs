using System;
using System.Collections;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class StatelessCompareHintTests : CompareHintTestBase<StatelessCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly StatelessCompareHint s_TestInstance = new StatelessCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(StatelessSample) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes
            = new[] { typeof(object), typeof(string), typeof(IList), typeof(int) };

        /// <summary>Sets up the tests.</summary>
        public StatelessCompareHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }

        /// <summary>Verifies the hint supports the correct types.</summary>
        public override void TryCompare_SupportsDifferentValidTypes()
        {
            // Stateless objects can't be different.
        }
    }
}
