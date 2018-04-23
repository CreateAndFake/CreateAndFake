using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
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

        /// <summary>Verifies mismatched list sizes aren't equal.</summary>
        [Theory, RandomData]
        public void Compare_SizeMismatchOutOfBounds(List<string> original)
        {
            List<string> variant = Tools.Duplicator.Copy(original);
            variant.RemoveAt(variant.Count - 1);

            (bool, IEnumerable<Difference>) result = TestInstance.TryCompare(original, variant, CreateChainer());
            Tools.Asserter.Is(true, result.Item1);
            Tools.Asserter.HasCount(1, result.Item2);
            Tools.Asserter.Is(true, result.Item2.Single().ToString().Contains("outofbounds"));

            result = TestInstance.TryCompare(variant, original, CreateChainer());
            Tools.Asserter.Is(true, result.Item1);
            Tools.Asserter.HasCount(1, result.Item2);
            Tools.Asserter.Is(true, result.Item2.Single().ToString().Contains("outofbounds"));
        }
    }
}
