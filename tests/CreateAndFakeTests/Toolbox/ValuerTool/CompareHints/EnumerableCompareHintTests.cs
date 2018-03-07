using System;
using System.Collections;
using System.Collections.Generic;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class EnumerableCompareHintTests : CompareHintTestBase<EnumerableCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly EnumerableCompareHint s_TestInstance = new EnumerableCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes
            = new[] { typeof(IEnumerable), typeof(IList), typeof(List<string>), typeof(List<KeyValuePair<int, int>>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes
            = new[] { typeof(object), typeof(int) };

        /// <summary>Sets up the tests.</summary>
        public EnumerableCompareHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
