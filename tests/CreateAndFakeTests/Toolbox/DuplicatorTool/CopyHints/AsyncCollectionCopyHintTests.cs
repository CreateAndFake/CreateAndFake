using System;
using System.Collections;
using System.Collections.Generic;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class AsyncCollectionCopyHintTests : CopyHintTestBase<AsyncCollectionCopyHint>
    {
        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes
            = new[] { typeof(IAsyncEnumerable<int>), typeof(IAsyncEnumerable<string>), typeof(IAsyncEnumerable<object>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(object), typeof(IEnumerable) };

        /// <summary>Sets up the tests.</summary>
        public AsyncCollectionCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }
    }
}
