using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class Behavior_T_Tests
    {
        /// <summary>Verifies behavior behavior.</summary>
        [TestMethod]
        public void Error_BehaviorWorks()
        {
            Tools.Asserter.Throws<NotImplementedException>(
                () => Behavior<string>.Error().Invoke(Array.Empty<object>()));
        }

        /// <summary>Verifies behavior behavior.</summary>
        [TestMethod]
        public void Throw_BehaviorWorks()
        {
            Tools.Asserter.Throws<InvalidOperationException>(
                () => Behavior<string>.Throw<InvalidOperationException>().Invoke(Array.Empty<object>()));
        }
    }
}
