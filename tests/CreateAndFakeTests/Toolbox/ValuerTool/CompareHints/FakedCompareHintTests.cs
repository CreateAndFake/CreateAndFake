using System;
using System.Collections;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class FakedCompareHintTests : CompareHintTestBase<FakedCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly FakedCompareHint _TestInstance = new FakedCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(IFaked) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(IEnumerable), typeof(string), typeof(int) };

        /// <summary>Sets up the tests.</summary>
        public FakedCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
    }
}
