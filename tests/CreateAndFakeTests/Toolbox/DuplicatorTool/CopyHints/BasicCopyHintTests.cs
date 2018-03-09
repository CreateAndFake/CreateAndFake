using System;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class BasicCopyHintTests : CopyHintTestBase<BasicCopyHint>
    {
        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes
            = new[] { typeof(BindingFlags), typeof(string), typeof(int) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(DataHolderSample) };

        /// <summary>Sets up the tests.</summary>
        public BasicCopyHintTests() : base(s_ValidTypes, s_InvalidTypes, true) { }

        /// <summary>Verifies the hint can handle just plain objects.</summary>
        [TestMethod]
        public void TryCopy_HandlesBaseObject()
        {
            object data = new object();
            (bool, object) result = new BasicCopyHint().TryCopy(data, CreateChainer());

            Tools.Asserter.Is((true, data), result);
            Tools.Asserter.ReferenceEqual(data, result.Item2);
        }
    }
}
