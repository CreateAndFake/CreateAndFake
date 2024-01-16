using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool;

/// <summary>Verifies behavior.</summary>
public static class Behavior_T_Tests
{
    [Fact]
    internal static void Error_BehaviorWorks()
    {
        Tools.Asserter.Throws<NotImplementedException>(
            () => Behavior<string>.Error().Invoke([]));
    }

    [Fact]
    internal static void Throw_BehaviorWorks()
    {
        Tools.Asserter.Throws<InvalidOperationException>(
            () => Behavior<string>.Throw<InvalidOperationException>().Invoke([]));
    }
}
