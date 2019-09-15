using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class FakedCompareHintTests : CompareHintTestBase<FakedCompareHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly FakedCompareHint _TestInstance = new FakedCompareHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(IFaked) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(IEnumerable), typeof(string), typeof(int) };

        /// <summary>Sets up the tests.</summary>
        public FakedCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

        /// <summary>Verifies the hint supports the correct types.</summary>
        [Fact]
        public override void TryCompare_SupportsDifferentValidTypes()
        {
            try
            {
                base.TryCompare_SupportsDifferentValidTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                throw new InvalidOperationException(
                    "Reflection failure:" + ex.LoaderExceptions.Select(e => e.Message), ex);
            }
        }
    }
}
