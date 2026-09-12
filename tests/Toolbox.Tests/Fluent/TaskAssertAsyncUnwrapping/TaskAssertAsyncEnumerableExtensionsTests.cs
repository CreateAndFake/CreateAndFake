using System.Reflection;
using Werecodent.CreateAndFake.AsserterTool;
using Werecodent.CreateAndFake.Design.Content;
using Werecodent.CreateAndFake.Fluent.AssertAsyncCalls;

namespace Werecodent.CreateAndFake.Tests.Fluent.TaskAssertAsyncUnwrapping;

public static class TaskAssertAsyncEnumerableExtensionsTests
{
    [Fact]
    internal static Task TaskAssertAsyncEnumerableExtensions_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync(
            typeof(TaskAssertAsyncEnumerableExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }

    [Fact]
    internal static Task TaskAssertAsyncEnumerableExtensions_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync(
            typeof(TaskAssertAsyncEnumerableExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }

    [Fact]
    internal static void TaskAssertAsyncEnumerableExtensions_MatchesEveryMethod()
    {
        typeof(AssertAsyncEnumerableBase<,>)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .OrderBy(m => m.Name)
            .Select(m => m.Name)
            .Except([nameof(AssertAsyncEnumerableBase<,>.ThrowsAsync)])
            .Assert()
            .Is(
                typeof(TaskAssertAsyncEnumerableExtensions)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .OrderBy(m => m.Name)
                    .Select(m => m.Name)
                    .Except([nameof(TaskAssertAsyncEnumerableExtensions.ThrowsExceptionAsync)])
            );
    }

    [Theory, RandomData]
    internal static async Task TaskAssertAsyncEnumerableExtensions_ObjectExtensionsWork(
        IAsyncEnumerable<int> sampleA,
        IAsyncEnumerable<int> sampleB
    )
    {
        await Task.FromResult(sampleA.Assert()).Is(sampleA);
        await Task.FromResult(sampleA.Assert())
            .IsNotAsync(sampleB, TestContext.Current.CancellationToken);
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

        await Task.FromResult(valid.Assert()).IsEmptyAsync(canceler);
        await Task.FromResult(valid.Assert()).IsEmptyAsync(canceler, mod);
        await Task.FromResult(invalid.Assert())
            .IsEmptyAsync(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(invalid.Assert())
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

        await Task.FromResult(valid.Assert()).IsNotEmptyAsync(canceler);
        await Task.FromResult(valid.Assert()).IsNotEmptyAsync(canceler, mod);
        await Task.FromResult(invalid.Assert())
            .IsNotEmptyAsync(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(invalid.Assert())
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

        await Task.FromResult(data.Assert()).HasCountAsync(1, canceler);
        await Task.FromResult(data.Assert()).HasCountAsync(1, canceler, mod);
        await Task.FromResult(data.Assert())
            .HasCountAsync(0, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
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

        await Task.FromResult(data.Assert()).HasCountLessThanAsync(2, canceler);
        await Task.FromResult(data.Assert()).HasCountLessThanAsync(2, canceler, mod);
        await Task.FromResult(data.Assert())
            .HasCountLessThanAsync(1, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
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

        await Task.FromResult(data.Assert()).HasCountLessOrExactlyAsync(2, canceler);
        await Task.FromResult(data.Assert()).HasCountLessOrExactlyAsync(1, canceler, mod);
        await Task.FromResult(data.Assert())
            .HasCountLessOrExactlyAsync(0, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
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

        await Task.FromResult(data.Assert()).HasCountMoreThanAsync(0, canceler);
        await Task.FromResult(data.Assert()).HasCountMoreThanAsync(0, canceler, mod);
        await Task.FromResult(data.Assert())
            .HasCountMoreThanAsync(1, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
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

        await Task.FromResult(data.Assert()).HasCountMoreOrExactlyAsync(0, canceler);
        await Task.FromResult(data.Assert()).HasCountMoreOrExactlyAsync(1, canceler, mod);
        await Task.FromResult(data.Assert())
            .HasCountMoreOrExactlyAsync(2, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
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

        await Task.FromResult(data.Assert()).ContainsAsync(valid, canceler);
        await Task.FromResult(data.Assert()).ContainsAsync(valid, canceler, mod);
        await Task.FromResult(data.Assert())
            .ContainsAsync(invalid, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
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

        await Task.FromResult(data.Assert()).ContainsNotAsync(valid, canceler);
        await Task.FromResult(data.Assert()).ContainsNotAsync(valid, canceler, mod);
        await Task.FromResult(data.Assert())
            .ContainsNotAsync(invalid, canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
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

        await Task.FromResult(data.Assert(silentFailSet)).FailAsync(canceler);
        await Task.FromResult(data.Assert(silentFailSet)).FailAsync(canceler, mod);
        await Task.FromResult(data.Assert())
            .FailAsync(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
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

        await Task.FromResult(data.Assert(debugPassSet)).DebugAsync(canceler);
        await Task.FromResult(data.Assert(debugPassSet)).DebugAsync(canceler, mod);
        await Task.FromResult(data.Assert(debugFailSet))
            .DebugAsync(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert(debugFailSet))
            .DebugAsync(canceler, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task ThrowsExceptionAsync_Forwarded(
        IAsyncEnumerable<int> data,
        Exception error
    )
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

        await Task.FromResult(thrower().Assert()).ThrowsExceptionAsync(canceler);
        await Task.FromResult(thrower().Assert()).ThrowsExceptionAsync(canceler, mod);
        await Task.FromResult(data.Assert())
            .ThrowsExceptionAsync(canceler)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .ThrowsExceptionAsync(canceler, mod)
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
