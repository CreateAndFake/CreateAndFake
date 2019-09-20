using System;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class BasicCopyHintTests : CopyHintTestBase<BasicCopyHint>
    {
        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes
            = new[] { typeof(BindingFlags), typeof(string), typeof(int), typeof(decimal) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(DataHolderSample) };

        /// <summary>Sets up the tests.</summary>
        public BasicCopyHintTests() : base(_ValidTypes, _InvalidTypes, true) { }

        [Fact]
        internal static void TryCopy_HandlesBaseObject()
        {
            object data = new object();
            (bool, object) result = new BasicCopyHint().TryCopy(data, CreateChainer());

            Tools.Asserter.Is((true, data), result);
            Tools.Asserter.ReferenceEqual(data, result.Item2);
        }
    }
}
