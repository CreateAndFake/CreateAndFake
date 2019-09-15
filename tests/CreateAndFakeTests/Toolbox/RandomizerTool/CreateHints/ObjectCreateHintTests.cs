using System;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class ObjectCreateHintTests : CreateHintTestBase<ObjectCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ObjectCreateHint _TestInstance = new ObjectCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(object), typeof(DataHolderSample),
            typeof(IUnimplementedSample), typeof(FieldSample), typeof(FactorySample) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(Arg) };

        /// <summary>Sets up the tests.</summary>
        public ObjectCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
    }
}
