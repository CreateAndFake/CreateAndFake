using System;
using System.Collections.Generic;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class GenericCreateHintTests : CreateHintTestBase<GenericCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly GenericCreateHint _TestInstance = new GenericCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes
            = new[] { typeof(IList<>), typeof(KeyValuePair<,>), typeof(GenericSample<>), typeof(ConstraintSample<,>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes
            = new[] { typeof(object), typeof(IList<string>), typeof(KeyValuePair<int, int>) };

        /// <summary>Sets up the tests.</summary>
        public GenericCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
    }
}
