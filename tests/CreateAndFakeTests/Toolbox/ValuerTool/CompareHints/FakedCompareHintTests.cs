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
        private static readonly FakedCompareHint s_TestInstance = new FakedCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(IFaked) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(IEnumerable), typeof(string), typeof(int) };

        /// <summary>Sets up the tests.</summary>
        public FakedCompareHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
