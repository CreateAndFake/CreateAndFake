using System.Collections;
using Werecodent.CreateAndFake.AsserterTool;
using Werecodent.CreateAndFake.Fluent.AssertAsyncCalls;
using Werecodent.CreateAndFake.Fluent.AssertCalls;
using Werecodent.CreateAndFake.Fluent.Chaining;
using Werecodent.CreateAndFake.Samples.Scenarios;

namespace Werecodent.CreateAndFake.Tests.Fluent.TaskChainingUnwrapping;

public static class TaskResultChainerExtensionsTests
{
    [Fact]
    internal static Task TaskResultChainerExtensions_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync(
            typeof(TaskResultChainerExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }

    [Fact]
    internal static Task TaskResultChainerExtensions_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync(
            typeof(TaskResultChainerExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }

    [Theory, RandomData]
    internal static void That_SupportsIAsyncEnumerable(
        Task<ResultChainer<IAsyncEnumerable<int>>> chainer
    )
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertAsyncEnumerable<int>>));
    }

    [Theory, RandomData]
    internal static void That_SupportsObject(Task<ResultChainer<object>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertAsyncObject>));
    }

    [Theory, RandomData]
    internal static void That_SupportsUnknownObject(Task<ResultChainer<DataSample>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertAsyncObject>));
    }

    [Theory, RandomData]
    internal static void That_SupportsGenericTask(Task<ResultChainer<Task<string>>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertGenericTask<string>>));
    }

    [Theory, RandomData]
    internal static void That_SupportsGenericValueTask(Task<ResultChainer<ValueTask<int>>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertGenericValueTask<int>>));
    }

    [Theory, RandomData]
    internal static void That_SupportsNullableGenericValueTask(
        Task<ResultChainer<ValueTask<int>?>> chainer
    )
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertGenericValueTask<int>>));
    }

    [Theory, RandomData]
    internal static void That_SupportsTask(Task<ResultChainer<Task>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertTask>));
    }

    [Theory, RandomData]
    internal static void That_SupportsValueTask(Task<ResultChainer<ValueTask>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertValueTask>));
    }

    [Theory, RandomData]
    internal static void That_SupportsNullableValueTask(Task<ResultChainer<ValueTask?>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertValueTask>));
    }

    [Theory, RandomData]
    internal static void That_SupportsAction(Task<ResultChainer<Action>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertAction>));
    }

    [Theory, RandomData]
    internal static void That_SupportsIComparable(Task<ResultChainer<IComparable>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertComparable>));
    }

    /*[Theory, RandomData]
    internal static void That_SupportsInt(Task<ResultChainer<int>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertComparable>));
    }*/

    [Theory, RandomData]
    internal static void That_SupportsGenericIEnumerable(
        Task<ResultChainer<IEnumerable<string>>> chainer
    )
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertEnumerable>));
    }

    [Theory, RandomData]
    internal static void That_SupportsIEnumerable(Task<ResultChainer<IEnumerable>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertEnumerable>));
    }

    [Theory, RandomData]
    internal static void That_SupportsException(Task<ResultChainer<Exception>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertError>));
    }

    [Theory, RandomData]
    internal static void That_SupportsFunc(Task<ResultChainer<Func<int>>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertFunc<int>>));
    }

    [Theory, RandomData]
    internal static void That_SupportsString(Task<ResultChainer<string>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertString>));
    }

    [Theory, RandomData]
    internal static void That_SupportsType(Task<ResultChainer<Type>> chainer)
    {
        chainer.That().GetType().Assert().Is(typeof(Task<AssertType>));
    }
}
