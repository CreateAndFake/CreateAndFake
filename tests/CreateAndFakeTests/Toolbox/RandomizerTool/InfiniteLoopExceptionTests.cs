using CreateAndFake;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class InfiniteLoopExceptionTests : ExceptionTestBase<InfiniteLoopException>
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [TestMethod]
        public void InfiniteLoopException_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<InfiniteLoopException>();
        }
    }
}
