using System;
using System.Linq;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class CloneableCopyHintTests : CopyHintTestBase<CloneableCopyHint>
    {
        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = Enumerable.Repeat(typeof(ICloneable), 10).ToArray();

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public CloneableCopyHintTests() : base(s_ValidTypes, s_InvalidTypes) { }
    }
}
