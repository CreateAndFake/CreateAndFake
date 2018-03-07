using System;
using System.Collections;
using System.Linq;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class CollectionCopyHintTests : CopyHintTestBase<CollectionCopyHint>
    {
        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = CollectionCreateHint.PotentialCollections.Concat(new[]
        {
            typeof(int[]), typeof(string[]), typeof(ArrayList), typeof(Queue), typeof(Stack), typeof(Array)
        }).ToArray();

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public CollectionCopyHintTests() : base(s_ValidTypes, s_InvalidTypes) { }
    }
}
