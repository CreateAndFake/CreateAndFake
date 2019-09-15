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
        private static readonly StatelessCompareHint _TestInstance = new StatelessCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(StatelessSample) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes
            = new[] { typeof(object), typeof(string), typeof(IList), typeof(int) };

        /// <summary>Sets up the tests.</summary>
        public StatelessCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

        /// <summary>Verifies the hint supports the correct types.</summary>
        public override void TryCompare_SupportsDifferentValidTypes()
        {
            // Stateless objects can't be different.
        }
    }
}
