﻿using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFakeTests.Toolbox.ValuerTool;

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
        new Valuer(true, null).Assert().Pass();
        new Valuer(false, null).Assert().Pass();
    }

    [Fact]
    internal static void GetHashCode_MissingMatchThrows()
    {
        new Valuer(false)
            .Assert(v => v.GetHashCode((object)null))
            .Throws<NotSupportedException>();

        new Valuer(false)
            .Assert(v => v.GetHashCode(new object()))
            .Throws<NotSupportedException>();
    }

    [Theory, RandomData]
    internal static void GetHashCode_SupportsMultiple(int value1, int value2)
    {
        Tools.Valuer.GetHashCode(value1, value2).Assert().Is(Tools.Valuer.GetHashCode([value1, value2]));
    }

    [Theory, RandomData]
    internal static void GetHashCode_ValidHint(object data, int result, [Fake] CompareHint hint)
    {
        hint.ToFake().Setup("Supports",
            [data, data, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Returns(true, Times.Once));
        hint.ToFake().Setup("GetHashCode",
            [data, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Returns(result, Times.Once));

        new Valuer(false, hint).GetHashCode(data).Assert().Is(result);
        hint.VerifyAllCalls(Times.Exactly(2));
    }

    [Fact]
    internal static void Compare_MissingMatchThrows()
    {
        new Valuer(false).Assert(v => v.Compare(null, new object())).Throws<NotSupportedException>();
        new Valuer(false).Assert(v => v.Compare(new object(), new object())).Throws<NotSupportedException>();
    }

    [Theory, RandomData]
    internal static void Compare_ReferenceNoDifferences(object data)
    {
        new Valuer(false).Compare(data, data).Assert().IsEmpty();
    }

    [Theory, RandomData]
    internal static void Compare_NullableWorks(int? item)
    {
        item.Assert().IsNot(null);
        item.CreateVariant().Assert().IsNot(item);
        item.CreateDeepClone().Assert().Is(item);

        int? none = null;
        none.Assert().IsNot(item);
        none.Assert().Is(none);
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

        new Valuer(false, hint.Dummy).Equals(data1, data2).Assert().Is(true);
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

        new Valuer(false, hint.Dummy).Equals(data1, data2).Assert().Is(false);
        hint.VerifyAll(Times.Exactly(2));
    }

    [Theory, RandomData]
    internal static void Compare_InfiniteLoopDetails(object item1, object item2, Fake<CompareHint> hint)
    {
        hint.Setup("Supports",
            [item1, item2, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

        new Valuer(false, hint.Dummy)
            .Assert(v => v.Compare(item1, item2))
            .Throws<InsufficientExecutionStackException>().Message
            .Assert()
            .Contains(item1.GetType().Name);
    }

    [Theory, RandomData]
    internal static void GetHashCode_InfiniteLoopDetails(object item, Fake<CompareHint> hint)
    {
        hint.Setup("Supports",
            [item, item, Arg.LambdaAny<ValuerChainer>()],
            Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

        new Valuer(false, hint.Dummy)
            .Assert(v => v.GetHashCode(item))
            .Throws<InsufficientExecutionStackException>().Message
            .Assert()
            .Contains(item.GetType().Name);
    }

    [Fact]
    internal static void GetHashCode_CanNotSupportNull()
    {
        new Valuer(false)
            .Assert(v => v.GetHashCode((object)null))
            .Throws<NotSupportedException>();
    }

    [Fact]
    internal static void Compare_CanNotSupportNull()
    {
        new Valuer(false)
            .Assert(v => v.Compare(null, new object()))
            .Throws<NotSupportedException>();
    }
}
