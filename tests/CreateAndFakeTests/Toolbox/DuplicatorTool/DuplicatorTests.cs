using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool;

public static class DuplicatorTests
{
    [Fact]
    internal static void Duplicator_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<Duplicator>();
    }

    [Fact]
    internal static void New_NullHintsValid()
    {
        new Duplicator(Tools.Asserter, true, null).Assert().IsNot(null);
        new Duplicator(Tools.Asserter, false, null).Assert().IsNot(null);
    }

    [Fact]
    internal static void Copy_MissingMatchThrows()
    {
        new Duplicator(Tools.Asserter, false)
            .Assert(d => d.Copy(new object()))
            .Throws<NotSupportedException>();
    }

    [Fact]
    internal static void Copy_NullWorks()
    {
        new Duplicator(Tools.Asserter, false).Copy<object>(null).Assert().Is(null);
    }

    [Theory, RandomData]
    internal static void Copy_ValidHintWorks(object data, [Stub] CopyHint hint)
    {
        hint.TryCopy(data, Arg.Any<DuplicatorChainer>()).SetupReturn((true, data), Times.Once);

        new Duplicator(Tools.Asserter, false, hint).Copy(data).Assert().Is(data);

        hint.VerifyAllCalls(Times.Once);
    }

    [Theory, RandomData]
    internal static void Copy_InfiniteLoopDetails(object instance, [Stub] CopyHint hint)
    {
        hint.ToFake().Setup(
            m => m.TryCopy(instance, Arg.Any<DuplicatorChainer>()),
            Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

        new Duplicator(Tools.Asserter, false, hint)
            .Assert(d => d.Copy(instance))
            .Throws<InsufficientExecutionStackException>().Message
            .Assert().Contains(instance.GetType().Name);
    }
}
