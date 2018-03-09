using System;
using System.Collections.Generic;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class ObjectCompareHintTests : CompareHintTestBase<ObjectCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ObjectCompareHint s_TestInstance
            = new ObjectCompareHint(BindingFlags.Public | BindingFlags.Instance);

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes
            = new[] { typeof(object), typeof(DataHolderSample), typeof(FieldSample) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = Type.EmptyTypes;

        /// <summary>Sets up the tests.</summary>
        public ObjectCompareHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }

        /// <summary>Verifies the hint finds differences comparing different objects.</summary>
        [TestMethod]
        public void Compare_DifferentObjectsDifferences()
        {
            var expected = new { Value = Tools.Randomizer.Create<string>() };
            var actual = new { Value = Tools.Randomizer.Create<string>() };

            (bool, IEnumerable<Difference>) result = TestInstance
                .TryCompare(expected, actual, CreateCompareChainer());

            Tools.Asserter.Is(true, result.Item1);
            Tools.Asserter.IsNotEmpty(result.Item2);
        }
    }
}
