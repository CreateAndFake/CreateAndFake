using System;
using System.Collections;
using System.Collections.Generic;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class AsyncEnumerableCompareHintTests : CompareHintTestBase<AsyncEnumerableCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly AsyncEnumerableCompareHint _TestInstance = new();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes
            = new[] { typeof(IAsyncEnumerable<int>), typeof(IAsyncEnumerable<string>), typeof(IAsyncEnumerable<object>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(IEnumerable), typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public AsyncEnumerableCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
    }
}
