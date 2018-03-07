using System;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class ObjectCreateHintTests : CreateHintTestBase<ObjectCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ObjectCreateHint s_TestInstance = new ObjectCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(object), typeof(DataHolderSample),
            typeof(IUnimplementedSample), typeof(FieldSample), typeof(FactorySample) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(Arg) };

        /// <summary>Sets up the tests.</summary>
        public ObjectCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
