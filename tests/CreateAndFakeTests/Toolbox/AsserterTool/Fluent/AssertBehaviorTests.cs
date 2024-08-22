using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

/// <summary>Verifies behavior.</summary>
public static class AssertBehaviorTests
{
    [Fact]
    internal static void AssertBehavior_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertBehavior>();
    }

    [Fact]
    internal static void AssertBehavior_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertBehavior>();
    }

    [Theory, RandomData]
    internal static void Throws_ReturnsException_Success(Exception error)
    {
        error.Assert(e => throw e).Throws<Exception>().Assert().Is(error);
    }

    [Theory, RandomData]
    internal static void Throws_CatchesExpected_Success(ArgumentNullException error)
    {
        error.Assert(e => throw e).Throws<ArgumentNullException>().Assert().Is(error);
    }

    [Theory, RandomData]
    internal static void Throws_UnwrapsAggregate_Success(InvalidOperationException error)
    {
        error.Assert(e => throw new AggregateException(e)).Throws<InvalidOperationException>().Assert().Is(error);
    }

    [Theory, RandomData]
    internal static void Throws_ActionNoException_Failure(Action behavior)
    {
        behavior.Assert(d => d.Assert().Throws<Exception>()).Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void Throws_FuncNoException_Failure(Func<object> behavior)
    {
        behavior.Assert(d => d.Assert().Throws<Exception>()).Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void Throws_WrongException_Failure(ArgumentNullException error)
    {
        error.Assert(e => e.Assert(ex => throw ex).Throws<InvalidOperationException>()).Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void Throws_WrongAggregate_Failure(InvalidOperationException error)
    {
        error
            .Assert(e => e.Assert(ex => throw new AggregateException(ex)).Throws<ArgumentNullException>())
            .Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void Throws_TooManyAggregate_Failure(ArgumentNullException error, InvalidOperationException error2)
    {
        error
            .Assert(e => error2.Assert(ex => throw new AggregateException(e, ex)).Throws<ArgumentNullException>())
            .Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void ThrowsNoException_NoopAction_Success(Action behavior)
    {
        behavior.Assert().ThrowsNoException();
    }

    [Theory, RandomData]
    internal static void ThrowsNoException_NoopFunc_Success(Func<object> behavior)
    {
        behavior.Assert().ThrowsNoException();
    }

    [Theory, RandomData]
    internal static void ThrowsNoException_Error_Failure(Exception error)
    {
        error.Assert(e => e.Assert(ex => throw ex).ThrowsNoException()).Throws<AssertException>();
    }
}
