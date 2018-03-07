using System;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class DelegateCreateHintTests : CreateHintTestBase<DelegateCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly DelegateCreateHint s_TestInstance = new DelegateCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(Delegate),
            typeof(Action), typeof(Action<string, object, int>), typeof(Func<int, string, object>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public DelegateCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
