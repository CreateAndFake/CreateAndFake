using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class EarlyFailCompareHintTests : CompareHintTestBase<EarlyFailCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly EarlyFailCompareHint s_TestInstance = new EarlyFailCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes
            = new[] { typeof(int), typeof(string), typeof(BindingFlags), typeof(Type), typeof(Delegate) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes
            = new[] { typeof(IDictionary), typeof(IEnumerable) };

        /// <summary>Sets up the tests.</summary>
        public EarlyFailCompareHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }

        /// <summary>Verifies the hint supports nulls.</summary>
        [TestMethod]
        public override void TryCompare_NullBehaviorCheck()
        {
            Tools.Asserter.Is(true, TestInstance.TryCompare(null, new object(), CreateChainer()).Item1);
            Tools.Asserter.IsNotEmpty(TestInstance.TryCompare(null, new object(), CreateChainer()).Item2);

            Tools.Asserter.Is(true, TestInstance.TryCompare(null, null, CreateChainer()).Item1);
            Tools.Asserter.IsEmpty(TestInstance.TryCompare(null, null, CreateChainer()).Item2);

            Tools.Asserter.Is(true, TestInstance.TryCompare(new object(), null, CreateChainer()).Item1);
            Tools.Asserter.IsNotEmpty(TestInstance.TryCompare(
                new object(), null, CreateChainer()).Item2.ToArray());

            Tools.Asserter.Throws<ArgumentNullException>(
                () => TestInstance.TryCompare(new object(), new object(), null));
        }

        /// <summary>Verifies the hint handles nulls properly.</summary>
        [TestMethod]
        public override void TryGetHashCode_NullBehaviorCheck()
        {
            Tools.Asserter.Is(true, TestInstance.TryGetHashCode(null, CreateChainer()).Item1);
            Tools.Asserter.Is(ValueComparer.NullHash, TestInstance.TryGetHashCode(null, CreateChainer()).Item2);

            Tools.Asserter.Throws<ArgumentNullException>(
                () => TestInstance.TryGetHashCode(new object(), null));
        }
    }
}
