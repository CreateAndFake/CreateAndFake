using System;
using System.Collections;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class ValuerEquatableCompareHintTests : CompareHintTestBase<ValuerEquatableCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ValuerEquatableCompareHint _TestInstance = new ValuerEquatableCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes
            = new[] { typeof(IValuerEquatable), typeof(ValuerEquatableSample), typeof(PrivateValuerEquatableSample) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(IDictionary), typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public ValuerEquatableCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
    }
}
