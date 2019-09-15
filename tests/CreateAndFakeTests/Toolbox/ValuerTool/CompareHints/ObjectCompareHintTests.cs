using System;
using System.Collections.Generic;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class ObjectCompareHintTests : CompareHintTestBase<ObjectCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ObjectCompareHint _TestInstance
            = new ObjectCompareHint(BindingFlags.Public | BindingFlags.Instance);

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes
            = new[] { typeof(object), typeof(DataHolderSample), typeof(FieldSample) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = Type.EmptyTypes;

        /// <summary>Sets up the tests.</summary>
        public ObjectCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

        [Fact]
        internal void Compare_DifferentObjectsDifferences()
        {
            var expected = new { Value = Tools.Randomizer.Create<string>() };
            var actual = new { Value = Tools.Randomizer.Create<string>() };

            (bool, IEnumerable<Difference>) result = TestInstance
                .TryCompare(expected, actual, CreateChainer());

            Tools.Asserter.Is(true, result.Item1);
            Tools.Asserter.IsNotEmpty(result.Item2);
        }
    }
}
