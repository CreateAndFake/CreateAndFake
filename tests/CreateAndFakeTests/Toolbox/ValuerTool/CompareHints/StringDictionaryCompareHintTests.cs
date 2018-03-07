using System;
using System.Collections;
using System.Collections.Specialized;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class StringDictionaryCompareHintTests : CompareHintTestBase<StringDictionaryCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly StringDictionaryCompareHint s_TestInstance = new StringDictionaryCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(StringDictionary) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(IDictionary), typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public StringDictionaryCompareHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
