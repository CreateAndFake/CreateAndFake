using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool;

/// <summary>Verifies behavior.</summary>
public static class ValuerTests
{
    [Fact]
    internal static void Valuer_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException(Tools.Valuer);
    }

    [Fact]
    internal static void New_NullHintsValid()
    {
        Tools.Asserter.IsNot(null, new Valuer(true, null));
        Tools.Asserter.IsNot(null, new Valuer(false, null));
    }

    [Fact]
    internal static void GetHashCode_MissingMatchThrows()
    {
        _ = Tools.Asserter.Throws<NotSupportedException>(
            () => new Valuer(false).GetHashCode((object)null));

        _ = Tools.Asserter.Throws<NotSupportedException>(
            () => new Valuer(false).GetHashCode(new object()));
    }

    [Fact]
    internal static void GetHashCode_SupportsMultiple()
    {
        int[] data = [Tools.Randomizer.Create<int>(), Tools.Randomizer.Create<int>()];

        Tools.Asserter.Is(Tools.Valuer.GetHashCode(data), Tools.Valuer.GetHashCode(data[0], data[1]));
    }

    [Theory, RandomData]
    internal static void GetHashCode_ValidHint(object data, int result, Fake<CompareHint> hint)
    {
        hint.Setup("Supports",
            [data, data, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Returns(true, Times.Once));
        hint.Setup("GetHashCode",
            [data, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Returns(result, Times.Once));

        Tools.Asserter.Is(result, new Valuer(false, hint.Dummy).GetHashCode(data));
        hint.VerifyAll(Times.Exactly(2));
    }

    [Fact]
    internal static void Compare_MissingMatchThrows()
    {
        _ = Tools.Asserter.Throws<NotSupportedException>(
            () => new Valuer(false).Compare(null, new object()));

        _ = Tools.Asserter.Throws<NotSupportedException>(
            () => new Valuer(false).Compare(new object(), new object()));
    }

    [Theory, RandomData]
    internal static void Compare_ReferenceNoDifferences(object data)
    {
        Tools.Asserter.IsEmpty(new Valuer(false).Compare(data, data));
    }

    [Theory, RandomData]
    internal static void Compare_NullableWorks(int? item)
    {
        Tools.Asserter.IsNot(item, Tools.Mutator.Variant(item));
        Tools.Asserter.Is(item, Tools.Duplicator.Copy(item));
        Tools.Asserter.IsNot(item, null);
        int? none = null;
        Tools.Asserter.IsNot(item, none);
        Tools.Asserter.Is(none, none);
    }

    [Theory, RandomData]
    internal static void Equals_NoDifferencesTrue(object data1, object data2, Fake<CompareHint> hint)
    {
        hint.Setup("Supports",
            [data1, data2, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Returns(true, Times.Once));
        hint.Setup("Compare",
            [data1, data2, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));

        Tools.Asserter.Is(true, new Valuer(false, hint.Dummy).Equals(data1, data2));

        hint.VerifyAll(Times.Exactly(2));
    }

    [Theory, RandomData]
    internal static void Equals_DifferencesFalse(object data1, object data2, Fake<CompareHint> hint)
    {
        hint.Setup("Supports",
            [data1, data2, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Returns(true, Times.Once));
        hint.Setup("Compare",
            [data1, data2, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>(), Times.Once));

        Tools.Asserter.Is(false, new Valuer(false, hint.Dummy).Equals(data1, data2));

        hint.VerifyAll(Times.Exactly(2));
    }

    [Theory, RandomData]
    internal static void Compare_InfiniteLoopDetails(object item1, object item2, Fake<CompareHint> hint)
    {
        hint.Setup("Supports",
            [item1, item2, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

        InsufficientExecutionStackException e = Tools.Asserter.Throws<InsufficientExecutionStackException>(
            () => new Valuer(false, hint.Dummy).Compare(item1, item2));

        Tools.Asserter.Is(true, e.Message.Contains(item1.GetType().Name));
    }

    [Theory, RandomData]
    internal static void GetHashCode_InfiniteLoopDetails(object item, Fake<CompareHint> hint)
    {
        hint.Setup("Supports",
            [item, item, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

        InsufficientExecutionStackException e = Tools.Asserter.Throws<InsufficientExecutionStackException>(
            () => new Valuer(false, hint.Dummy).GetHashCode(item));

        Tools.Asserter.Is(true, e.Message.Contains(item.GetType().Name));
    }

    [Fact]
    internal static void GetHashCode_CanNotSupportNull()
    {
        _ = Tools.Asserter.Throws<NotSupportedException>(() => new Valuer(false).GetHashCode((object)null));
    }

    [Fact]
    internal static void Compare_CanNotSupportNull()
    {
        _ = Tools.Asserter.Throws<NotSupportedException>(() => new Valuer(false).Compare(null, new object()));
    }
}
