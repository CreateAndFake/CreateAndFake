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
        private static readonly EnumCreateHint _TestInstance = new EnumCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(BindingFlags) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public EnumCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
    }
}
