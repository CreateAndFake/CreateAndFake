using System;
using System.Collections;
using System.Collections.Specialized;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class StringDictionaryCompareHintTests : CompareHintTestBase<StringDictionaryCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly StringDictionaryCompareHint _TestInstance = new();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(StringDictionary) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(IDictionary), typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public StringDictionaryCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
    }
}
