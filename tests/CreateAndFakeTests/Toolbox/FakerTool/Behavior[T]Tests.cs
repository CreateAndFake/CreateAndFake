using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.Toolbox.FakerTool;

public static class Behavior_T_Tests
{
    [Fact]
    internal static void Error_BehaviorWorks()
    {
        Behavior<string>.Error().Assert(b => b.Invoke([])).Throws<NotImplementedException>();
    }

    [Fact]
    internal static void Throw_BehaviorWorks()
    {
        Behavior<string>
            .Throw<InvalidOperationException>()
            .Assert(b => b.Invoke([]))
            .Throws<InvalidOperationException>();
    }
}
