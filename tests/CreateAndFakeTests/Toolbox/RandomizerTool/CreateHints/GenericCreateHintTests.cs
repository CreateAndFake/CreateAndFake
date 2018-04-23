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
        private static readonly GenericCreateHint s_TestInstance = new GenericCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes
            = new[] { typeof(IList<>), typeof(KeyValuePair<,>), typeof(GenericSample<>), typeof(ConstraintSample<,>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes
            = new[] { typeof(object), typeof(IList<string>), typeof(KeyValuePair<int, int>) };

        /// <summary>Sets up the tests.</summary>
        public GenericCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
