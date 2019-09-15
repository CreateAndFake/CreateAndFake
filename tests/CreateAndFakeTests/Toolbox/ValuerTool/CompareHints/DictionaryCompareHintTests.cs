using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class DictionaryCompareHintTests : CompareHintTestBase<DictionaryCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly DictionaryCompareHint _TestInstance = new DictionaryCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes
            = new[] { typeof(IDictionary), typeof(Dictionary<string, int>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes
            = new[] { typeof(object), typeof(string), typeof(IList), typeof(int) };

        /// <summary>Sets up the tests.</summary>
        public DictionaryCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Theory, RandomData]
        public void TryCompare_SameKeyDifferentValuesWorks(Dictionary<string, int> data)
        {
            Dictionary<string, int> dupe = Tools.Duplicator.Copy(data);
            string key = data.First().Key;
            dupe[key] = Tools.Mutator.Variant(data[key]);

            Tools.Asserter.IsNotEmpty(TestInstance.TryCompare(data, dupe, CreateChainer()).Item2.ToArray(),
                "Hint didn't find differences with a modified key on '" + data.GetType().Name + "'.");
        }
    }
}
