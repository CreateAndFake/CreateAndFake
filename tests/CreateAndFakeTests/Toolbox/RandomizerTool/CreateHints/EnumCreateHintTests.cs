using System;
using System.Reflection;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class EnumCreateHintTests : CreateHintTestBase<EnumCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly EnumCreateHint s_TestInstance = new EnumCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(BindingFlags) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public EnumCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
