using Werecodent.CreateAndFake.AsserterTool;
using Werecodent.CreateAndFake.Design.Content;
using Werecodent.CreateAndFake.Design.Exceptions;
using Werecodent.CreateAndFake.Design.Extensions;
using Werecodent.CreateAndFake.FakerTool;
using Werecodent.CreateAndFake.Fluent.AssertAsyncCalls;
using Werecodent.CreateAndFake.Fluent.Chaining;
using Werecodent.CreateAndFake.RunnerTool;

namespace Werecodent.CreateAndFake.Tests.Fluent.AssertAsyncCalls;

public static class AssertAsyncEnumerableTests
{
    [Fact]
    internal static Task AssertAsyncEnumerable_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync(
            typeof(AssertAsyncEnumerable<>),
            TestContext.Current.CancellationToken,
            opt =>
                opt with
                {
                    IgnorableExceptions = [typeof(AssertException), typeof(ToolException)],
                }
        );
    }

    [Fact]
    internal static Task AssertAsyncEnumerable_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync(
            typeof(AssertAsyncEnumerable<>),
            TestContext.Current.CancellationToken,
            opt =>
                opt with
                {
                    IgnorableExceptions = [typeof(AssertException), typeof(ToolException)],
                }
        );
    }

    [Theory, RandomData]
    internal static async Task AssertAsyncEnumerable_CallsAndChains(
        Injected<AssertAsyncEnumerable<int>> instance
    )
    {
        RunResults results = await Tools.Runner.CallMethodsOnAsync(
            instance.Dummy,
            TestContext.Current.CancellationToken,
            opt => opt with { IncludeBaseObjectMethods = false }
        );
        results
            .RawResults.Where(r => r.Result != null)
            .Where(r =>
                r.Result
                    is not (
                        AssertChainer<AssertAsyncEnumerable<int>>
                        or Task<AssertChainer<AssertAsyncEnumerable<int>>>
                    )
            )
            .Where(r => !r.Result.GetType().Inherits(typeof(ExceptionChainer<>)))
            .Where(r => r.Result as string != "AssertAsyncEnumerable<Int32>")
            .OrderBy(r => r.Method.Name)
            .Assert()
            .IsEmpty();
    }

    [Theory, RandomData]
    internal static Task AssertAsyncEnumerable_ObjectExtensionsWork(
        IAsyncEnumerable<int> sampleA,
        IAsyncEnumerable<int> sampleB
    )
    {
        sampleA.Assert().Is(sampleA);
        return sampleA.Assert().IsNotAsync(sampleB, TestContext.Current.CancellationToken);
    }

    [Theory, RandomData]
    internal static async Task IsEmptyAsync_Forwarded(
        [Size(0)] IAsyncEnumerable<int> valid,
        [Size(1)] IAsyncEnumerable<int> invalid
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await valid.Assert().IsEmptyAsync(canceler);
        await valid.Assert().IsEmptyAsync(canceler, mod);
        await invalid
            .Assert()
            .IsEmptyAsync(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await invalid
            .Assert()
            .IsEmptyAsync(canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task IsNotEmptyAsync_Forwarded(
        [Size(1)] IAsyncEnumerable<int> valid,
        [Size(0)] IAsyncEnumerable<int> invalid
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await valid.Assert().IsNotEmptyAsync(canceler);
        await valid.Assert().IsNotEmptyAsync(canceler, mod);
        await invalid
            .Assert()
            .IsNotEmptyAsync(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await invalid
            .Assert()
            .IsNotEmptyAsync(canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountAsync_Forwarded([Size(1)] IAsyncEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await data.Assert().HasCountAsync(1, canceler);
        await data.Assert().HasCountAsync(1, canceler, mod);
        await data.Assert()
            .HasCountAsync(0, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .HasCountAsync(2, canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountLessThanAsync_Forwarded([Size(1)] IAsyncEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await data.Assert().HasCountLessThanAsync(2, canceler);
        await data.Assert().HasCountLessThanAsync(2, canceler, mod);
        await data.Assert()
            .HasCountLessThanAsync(1, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .HasCountLessThanAsync(0, canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountLessOrExactlyAsync_Forwarded(
        [Size(1)] IAsyncEnumerable<int> data
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await data.Assert().HasCountLessOrExactlyAsync(2, canceler);
        await data.Assert().HasCountLessOrExactlyAsync(1, canceler, mod);
        await data.Assert()
            .HasCountLessOrExactlyAsync(0, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .HasCountLessOrExactlyAsync(0, canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountMoreThanAsync_Forwarded([Size(1)] IAsyncEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await data.Assert().HasCountMoreThanAsync(0, canceler);
        await data.Assert().HasCountMoreThanAsync(0, canceler, mod);
        await data.Assert()
            .HasCountMoreThanAsync(1, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .HasCountMoreThanAsync(2, canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountMoreOrExactlyAsync_Forwarded(
        [Size(1)] IAsyncEnumerable<int> data
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await data.Assert().HasCountMoreOrExactlyAsync(0, canceler);
        await data.Assert().HasCountMoreOrExactlyAsync(1, canceler, mod);
        await data.Assert()
            .HasCountMoreOrExactlyAsync(2, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .HasCountMoreOrExactlyAsync(2, canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task ContainsAsync_Forwarded(int valid, int invalid)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        IAsyncEnumerable<int> data = AsyncSeriesHelper.CreateFromAsync([valid], 1, canceler);

        await data.Assert().ContainsAsync(valid, canceler);
        await data.Assert().ContainsAsync(valid, canceler, mod);
        await data.Assert()
            .ContainsAsync(invalid, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .ContainsAsync(invalid, canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task ContainsNotAsync_Forwarded(int valid, int invalid)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        IAsyncEnumerable<int> data = AsyncSeriesHelper.CreateFromAsync([invalid], 1, canceler);

        await data.Assert().ContainsNotAsync(valid, canceler);
        await data.Assert().ContainsNotAsync(valid, canceler, mod);
        await data.Assert()
            .ContainsNotAsync(invalid, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .ContainsNotAsync(invalid, canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task FailAsync_Forwarded(
        IAsyncEnumerable<int> data,
        [Fake] IAsserter asserter
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        ToolSet silentFailSet = MakeSet(asserter);

        await data.Assert(silentFailSet).FailAsync(canceler);
        await data.Assert(silentFailSet).FailAsync(canceler, mod);
        await data.Assert().FailAsync(canceler).Assert().ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .FailAsync(canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(1);
    }

    [Theory, RandomData]
    internal static async Task DebugAsync_Forwarded(IAsyncEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        ToolSet debugPassSet = MakeSet(
            Tools.Asserter.WithOptions(opt => opt with { DebugAssertsFail = false })
        );
        ToolSet debugFailSet = MakeSet(
            Tools.Asserter.WithOptions(opt => opt with { DebugAssertsFail = true })
        );

        await data.Assert(debugPassSet).DebugAsync(canceler);
        await data.Assert(debugPassSet).DebugAsync(canceler, mod);
        await data.Assert(debugFailSet)
            .DebugAsync(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert(debugFailSet)
            .DebugAsync(canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task ThrowsAsync_Forwarded(IAsyncEnumerable<int> data, Exception error)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        async IAsyncEnumerable<int> thrower()
        {
            yield return 1;
            await Task.Delay(0, canceler).ConfigureAwait(false);
            throw error;
        }

        await thrower().Assert().ThrowsAsync<Exception>(canceler);
        await thrower().Assert().ThrowsAsync<Exception>(canceler, mod);
        await data.Assert()
            .ThrowsAsync<Exception>(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await data.Assert()
            .ThrowsAsync<Exception>(canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    private static ToolSet MakeSet(IAsserter asserter)
    {
        return new(
            Tools.Gen,
            Tools.Valuer,
            Tools.Faker,
            Tools.Randomizer,
            Tools.Extractor,
            Tools.Mutator,
            asserter,
            Tools.Duplicator,
            Tools.Runner,
            Tools.Tester
        );
    }
}
