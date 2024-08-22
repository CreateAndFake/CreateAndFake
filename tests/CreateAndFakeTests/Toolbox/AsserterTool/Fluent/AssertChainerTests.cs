using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

public static class AssertChainerTests
{
    [Fact]
    internal static void AssertChainer_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertChainer<object>>();
    }

    [Fact]
    internal static void AssertChainer_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertChainer<object>>();
    }

    [Theory, RandomData]
    internal static void And_ReturnsInput_Success(object data)
    {
        new AssertChainer<object>(data, Tools.Gen, Tools.Valuer).And.Assert().Is(data);
    }

    [Theory, RandomData]
    internal static void Also_HandlesObject_Success(AssertChainer<object> chainer, object data)
    {
        chainer.Also(data).GetType().Assert().Inherits<AssertObject>();
    }

    [Theory, RandomData]
    internal static void Also_HandlesCollection_Success(AssertChainer<object> chainer, IEnumerable<int> data)
    {
        chainer.Also(data).GetType().Assert().Inherits<AssertGroup>();
    }

    [Theory, RandomData]
    internal static void Also_HandlesString_Success(AssertChainer<object> chainer, string data)
    {
        chainer.Also(data).GetType().Assert().Inherits<AssertText>();
    }

    [Theory, RandomData]
    internal static void Also_HandlesComparables_Success(AssertChainer<object> chainer, IComparable data)
    {
        chainer.Also(data).GetType().Assert().Inherits<AssertComparable>();
    }

    [Theory, RandomData]
    internal static void Also_HandlesType_Success(AssertChainer<object> chainer, Type data)
    {
        chainer.Also(data).GetType().Assert().Inherits<AssertType>();
    }

    [Theory, RandomData]
    internal static void Also_HandlesAction_Success(AssertChainer<object> chainer, Action behavior)
    {
        chainer.Also(behavior).GetType().Assert().Inherits<AssertBehavior>();
    }

    [Theory, RandomData]
    internal static void Also_HandlesFunc_Success(AssertChainer<object> chainer, Func<string> behavior)
    {
        chainer.Also(behavior).GetType().Assert().Inherits<AssertBehavior>();
    }

    [Theory, RandomData]
    internal static void Also_HandlesCompiledAction_Success(AssertChainer<object> chainer, string data)
    {
        chainer.Also(data, d => d.VerifyAllCalls()).GetType().Assert().Inherits<AssertBehavior>();
    }

    [Theory, RandomData]
    internal static void Also_HandlesCompiledFunc_Success(AssertChainer<object> chainer, string data)
    {
        chainer.Also(data, d => d.Length).GetType().Assert().Inherits<AssertBehavior>();
    }
}
