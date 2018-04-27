using System;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class FakeCreateHintTests : CreateHintTestBase<FakeCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly FakeCreateHint s_TestInstance = new FakeCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(Fake<object>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public FakeCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
