using System;
using System.Collections;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class ValuerEquatableCompareHintTests : CompareHintTestBase<ValuerEquatableCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ValuerEquatableCompareHint s_TestInstance = new ValuerEquatableCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes
            = new[] { typeof(IValuerEquatable), typeof(ValuerEquatableSample), typeof(PrivateValuerEquatableSample) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(IDictionary), typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public ValuerEquatableCompareHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
