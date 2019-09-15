using System;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class ExceptionCreateHintTests : CreateHintTestBase<ExceptionCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ExceptionCreateHint _TestInstance = new ExceptionCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(Exception) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(object), typeof(FakeVerifyException) };

        /// <summary>Sets up the tests.</summary>
        public ExceptionCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
    }
}
