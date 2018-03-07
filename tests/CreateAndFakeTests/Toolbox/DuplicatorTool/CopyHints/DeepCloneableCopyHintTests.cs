using System;
using System.Linq;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class DeepCloneableCopyHintTests : CopyHintTestBase<DeepCloneableCopyHint>
    {
        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = Enumerable.Repeat(typeof(IDeepCloneable), 10).ToArray();

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public DeepCloneableCopyHintTests() : base(s_ValidTypes, s_InvalidTypes) { }
    }
}
