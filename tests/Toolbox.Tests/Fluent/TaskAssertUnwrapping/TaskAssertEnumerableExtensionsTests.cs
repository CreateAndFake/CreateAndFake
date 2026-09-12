using System.Reflection;
using Werecodent.CreateAndFake.AsserterTool;
using Werecodent.CreateAndFake.Fluent.AssertCalls;
using Werecodent.CreateAndFake.Samples.Scenarios;

namespace Werecodent.CreateAndFake.Tests.Fluent.TaskAssertUnwrapping;

public static class TaskAssertEnumerableExtensionsTests
{
    [Fact]
    internal static Task TaskAssertEnumerableExtensions_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync(
            typeof(TaskAssertEnumerableExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }

    [Fact]
    internal static Task TaskAssertEnumerableExtensions_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync(
            typeof(TaskAssertEnumerableExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }

    [Fact]
    internal static void TaskAssertEnumerableExtensions_MatchesEveryMethod()
    {
        typeof(AssertEnumerableBase<>)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .OrderBy(m => m.Name)
            .Select(m => m.Name)
            .Assert()
            .Is(
                typeof(TaskAssertEnumerableExtensions)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .OrderBy(m => m.Name)
                    .Select(m => m.Name)
            );
    }

    [Theory, RandomData]
    internal static async Task TaskAssertEnumerableExtensions_ObjectExtensionsWork(
        IEnumerable<int> sampleA,
        IEnumerable<int> sampleB
    )
    {
        await Task.FromResult(sampleA.Assert()).Is(sampleA);
        await Task.FromResult(sampleA.Assert())
            .IsNotAsync(sampleB, TestContext.Current.CancellationToken);
    }

    [Theory, RandomData]
    internal static async Task IsEmpty_Forwarded(
        [Size(0)] IEnumerable<int> valid,
        [Size(1)] IEnumerable<int> invalid
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await Task.FromResult(valid.Assert()).IsEmpty();
        await Task.FromResult(valid.Assert()).IsEmpty(mod);
        await Task.FromResult(invalid.Assert())
            .IsEmpty()
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(invalid.Assert())
            .IsEmpty(mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task IsNotEmpty_Forwarded(
        [Size(1)] IEnumerable<int> valid,
        [Size(0)] IEnumerable<int> invalid
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await Task.FromResult(valid.Assert()).IsNotEmpty();
        await Task.FromResult(valid.Assert()).IsNotEmpty(mod);
        await Task.FromResult(invalid.Assert())
            .IsNotEmpty()
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(invalid.Assert())
            .IsNotEmpty(mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCount_Forwarded([Size(1)] IEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await Task.FromResult(data.Assert()).HasCount(1);
        await Task.FromResult(data.Assert()).HasCount(1, mod);
        await Task.FromResult(data.Assert())
            .HasCount(0)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .HasCount(2, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountLessThan_Forwarded([Size(1)] IEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await Task.FromResult(data.Assert()).HasCountLessThan(2);
        await Task.FromResult(data.Assert()).HasCountLessThan(2, mod);
        await Task.FromResult(data.Assert())
            .HasCountLessThan(1)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .HasCountLessThan(0, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountLessOrExactly_Forwarded([Size(1)] IEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await Task.FromResult(data.Assert()).HasCountLessOrExactly(2);
        await Task.FromResult(data.Assert()).HasCountLessOrExactly(1, mod);
        await Task.FromResult(data.Assert())
            .HasCountLessOrExactly(0)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .HasCountLessOrExactly(0, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountMoreThan_Forwarded([Size(1)] IEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await Task.FromResult(data.Assert()).HasCountMoreThan(0);
        await Task.FromResult(data.Assert()).HasCountMoreThan(0, mod);
        await Task.FromResult(data.Assert())
            .HasCountMoreThan(1)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .HasCountMoreThan(2, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task HasCountMoreOrExactly_Forwarded([Size(1)] IEnumerable<int> data)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }

        await Task.FromResult(data.Assert()).HasCountMoreOrExactly(0);
        await Task.FromResult(data.Assert()).HasCountMoreOrExactly(1, mod);
        await Task.FromResult(data.Assert())
            .HasCountMoreOrExactly(2)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .HasCountMoreOrExactly(2, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task Contains_Forwarded(int valid, int invalid)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        IEnumerable<int> data = [valid];

        await Task.FromResult(data.Assert()).Contains(valid);
        await Task.FromResult(data.Assert()).Contains(valid, mod);
        await Task.FromResult(data.Assert())
            .Contains(invalid)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .Contains(invalid, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task ContainsAsync_Forwarded(
        AsyncDataSample valid,
        AsyncDataSample invalid
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        IEnumerable<AsyncDataSample> data = [valid];

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
    internal static async Task ContainsNot_Forwarded(int valid, int invalid)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        IEnumerable<int> data = [invalid];

        await Task.FromResult(data.Assert()).ContainsNot(valid);
        await Task.FromResult(data.Assert()).ContainsNot(valid, mod);
        await Task.FromResult(data.Assert())
            .ContainsNot(invalid)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .ContainsNot(invalid, mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(2);
    }

    [Theory, RandomData]
    internal static async Task ContainsNotAsync_Forwarded(
        AsyncDataSample valid,
        AsyncDataSample invalid
    )
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        IEnumerable<AsyncDataSample> data = [invalid];

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
    internal static async Task Fail_Forwarded(IEnumerable<int> data, [Fake] IAsserter asserter)
    {
        CancellationToken canceler = TestContext.Current.CancellationToken;
        int modCount = 0;
        AsserterOptions mod(AsserterOptions opt)
        {
            modCount++;
            return opt;
        }
        ToolSet silentFailSet = MakeSet(asserter);

        await Task.FromResult(data.Assert(silentFailSet)).Fail();
        await Task.FromResult(data.Assert(silentFailSet)).Fail(mod);
        await Task.FromResult(data.Assert()).Fail().Assert().ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert())
            .Fail(mod)
            .Assert()
            .ThrowsAsync<AssertException>(canceler);

        modCount.Assert().Is(1);
    }

    [Theory, RandomData]
    internal static async Task Debug_Forwarded(IEnumerable<int> data)
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

        await Task.FromResult(data.Assert(debugPassSet)).Debug();
        await Task.FromResult(data.Assert(debugPassSet)).Debug(mod);
        await Task.FromResult(data.Assert(debugFailSet))
            .Debug()
            .Assert()
            .ThrowsAsync<AssertException>(canceler);
        await Task.FromResult(data.Assert(debugFailSet))
            .Debug(mod)
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
