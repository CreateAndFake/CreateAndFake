using System;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool;

/// <summary>Verifies behavior.</summary>
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
        Tools.Asserter.IsNot(null, new Duplicator(Tools.Asserter, true, null));
        Tools.Asserter.IsNot(null, new Duplicator(Tools.Asserter, false, null));
    }

    [Fact]
    internal static void Copy_MissingMatchThrows()
    {
        _ = Tools.Asserter.Throws<NotSupportedException>(
            () => new Duplicator(Tools.Asserter, false).Copy(new object()));
    }

    [Fact]
    internal static void Copy_NullWorks()
    {
        Tools.Asserter.Is(null, new Duplicator(Tools.Asserter, false).Copy<object>(null));
    }

    [Theory, RandomData]
    internal static void Copy_ValidHintWorks(object data, Fake<CopyHint> hint)
    {
        hint.Setup(
            m => m.TryCopy(data, Arg.Any<DuplicatorChainer>()),
            Behavior.Returns((true, data), Times.Once));

        object result = new Duplicator(Tools.Asserter, false, hint.Dummy).Copy(data);

        Tools.Asserter.CheckAll(
            () => Tools.Asserter.Is(data, result),
            () => hint.VerifyAll(Times.Once));
    }

    [Theory, RandomData]
    internal static void Copy_InfiniteLoopDetails(object instance, Fake<CopyHint> hint)
    {
        hint.Setup(
            m => m.TryCopy(instance, Arg.Any<DuplicatorChainer>()),
            Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

        InsufficientExecutionStackException e = Tools.Asserter.Throws<InsufficientExecutionStackException>(
            () => new Duplicator(Tools.Asserter, false, hint.Dummy).Copy(instance));

        Tools.Asserter.Is(true, e.Message.Contains(instance.GetType().Name));
    }
}
