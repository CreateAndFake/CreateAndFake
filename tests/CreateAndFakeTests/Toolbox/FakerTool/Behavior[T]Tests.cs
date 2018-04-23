using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class Behavior_T_Tests
    {
        /// <summary>Verifies behavior behavior.</summary>
        [Fact]
        public static void Error_BehaviorWorks()
        {
            Tools.Asserter.Throws<NotImplementedException>(
                () => Behavior<string>.Error().Invoke(Array.Empty<object>()));
        }

        /// <summary>Verifies behavior behavior.</summary>
        [Fact]
        public static void Throw_BehaviorWorks()
        {
            Tools.Asserter.Throws<InvalidOperationException>(
                () => Behavior<string>.Throw<InvalidOperationException>().Invoke(Array.Empty<object>()));
        }
    }
}
