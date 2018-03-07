using System;
using System.Linq;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class ValueCreateHintTests : CreateHintTestBase<ValueCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ValueCreateHint s_TestInstance = new ValueCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = ValueRandom.ValueTypes.ToArray();

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public ValueCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }
    }
}
