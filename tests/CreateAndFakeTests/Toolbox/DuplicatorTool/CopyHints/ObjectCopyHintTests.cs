using System;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class ObjectCopyHintTests : CopyHintTestBase<ObjectCopyHint>
    {
        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[]
        {
            typeof(object), typeof(IUnimplementedSample), typeof(DataHolderSample),
            typeof(FieldSample), typeof(PrivateValuerEquatableSample)
        };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(MismatchDataSample) };

        /// <summary>Sets up the tests.</summary>
        public ObjectCopyHintTests() : base(s_ValidTypes, s_InvalidTypes) { }
    }
}
