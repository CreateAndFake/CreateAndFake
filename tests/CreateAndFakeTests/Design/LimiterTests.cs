using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Design;
using Xunit;

namespace CreateAndFakeTests.Design;

#pragma warning disable xUnit1031 // Test methods should not use blocking code

/// <summary>Verifies behavior.</summary>
public static class LimiterTests
{
    /// <summary>Accuracy of wait resolution for delays.</summary>
    private const int _WaitAccuracy = 5;

    /// <summary>Small delay to test with.</summary>
    private static readonly TimeSpan _SmallDelay = new(0, 0, 0, 0, 20);

    [Fact]
    internal static void Limiter_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException(Limiter.Few);
    }

    [Fact]
    internal static async Task Repeat_AtLeastOnce()
    {
        int attempts = 0;

        _ = await new Limiter(0).Repeat("", () => attempts++).ConfigureAwait(true);
        Tools.Asserter.Is(1, attempts);

        _ = await new Limiter(TimeSpan.MinValue).Repeat("", () => attempts++).ConfigureAwait(true);
        Tools.Asserter.Is(2, attempts);
    }

    [Fact]
    internal static void StallUntil_AtLeastOnce()
    {
        int attempts = 0;

        _ = Tools.Asserter.Throws<TimeoutException>(
            () => new Limiter(0).StallUntil("", () => attempts++, () => false).Wait());
        Tools.Asserter.Is(1, attempts);

        _ = Tools.Asserter.Throws<TimeoutException>(
            () => new Limiter(TimeSpan.MinValue).StallUntil("", () => attempts++, () => false).Wait());
        Tools.Asserter.Is(2, attempts);
    }

    [Theory, RandomData]
    internal static void Retry_AtLeastOnce(Exception exception)
    {
        int attempts = 0;

        Tools.Asserter.Is(exception, Tools.Asserter.Throws<TimeoutException>(
            () => new Limiter(0).Retry("", () => { attempts++; throw exception; }).Wait()).InnerException);
        Tools.Asserter.Is(1, attempts);

        Tools.Asserter.Is(exception, Tools.Asserter.Throws<TimeoutException>(
            () => new Limiter(TimeSpan.MinValue).Retry("",
                () => { attempts++; throw exception; }).Wait()).InnerException);
        Tools.Asserter.Is(2, attempts);
    }

    [Theory, RandomData]
    internal static void Attempt_AtLeastOnce(Exception exception)
    {
        int attempts = 0;

        new Limiter(0).Attempt("", () => { attempts++; throw exception; }).Wait();
        Tools.Asserter.Is(1, attempts);

        new Limiter(TimeSpan.MinValue).Attempt("", () => { attempts++; throw exception; }).Wait();
        Tools.Asserter.Is(2, attempts);
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static async Task Repeat_TryLimited(int tries)
    {
        int attempts = 0;

        _ = await new Limiter(tries).Repeat("", () => attempts++).ConfigureAwait(true);
        Tools.Asserter.Is(tries, attempts);
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static void StallUntil_TryLimited(int tries)
    {
        int attempts = 0;

        _ = Tools.Asserter.Throws<TimeoutException>(
            () => new Limiter(tries).StallUntil("", () => attempts++, () => false).Wait());
        Tools.Asserter.Is(tries, attempts);
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static void Retry_TryLimited(int tries)
    {
        Exception exception = Tools.Randomizer.Create<Exception>();
        int attempts = 0;

        Tools.Asserter.Is(exception, Tools.Asserter.Throws<TimeoutException>(
            () => new Limiter(tries).Retry("", () => { attempts++; throw exception; }).Wait()).InnerException);
        Tools.Asserter.Is(tries, attempts);
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static void Attempt_TryLimited(int tries)
    {
        Exception exception = Tools.Randomizer.Create<Exception>();
        int attempts = 0;

        new Limiter(tries).Attempt("", () => { attempts++; throw exception; }).Wait();
        Tools.Asserter.Is(tries, attempts);
    }

    [Fact]
    internal static async Task Repeat_TimeoutLimited()
    {
        Stopwatch watch = Stopwatch.StartNew();
        await new Limiter(_SmallDelay).Repeat("", () => { }).ConfigureAwait(true);
        Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >= _SmallDelay.TotalMilliseconds - _WaitAccuracy);
    }

    [Fact]
    internal static void StallUntil_TimeoutLimited()
    {
        Stopwatch watch = Stopwatch.StartNew();

        _ = Tools.Asserter.Throws<TimeoutException>(
            () => new Limiter(_SmallDelay).StallUntil("", () => { }, () => false).Wait());
        Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >= _SmallDelay.TotalMilliseconds - _WaitAccuracy);
    }

    [Theory, RandomData]
    internal static void Retry_TimeoutLimited(Exception exception)
    {
        Stopwatch watch = Stopwatch.StartNew();

        Tools.Asserter.Is(exception, Tools.Asserter.Throws<TimeoutException>(
            () => new Limiter(_SmallDelay).Retry("", () => { throw exception; }).Wait()).InnerException);
        Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >= _SmallDelay.TotalMilliseconds - _WaitAccuracy);
    }

    [Theory, RandomData]
    internal static void Attempt_TimeoutLimited(Exception exception)
    {
        Stopwatch watch = Stopwatch.StartNew();

        Tools.Asserter.Is(null, new Limiter(_SmallDelay).Attempt("",
            () => watch.IsRunning ? throw exception : new object()).Result);
        Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >= _SmallDelay.TotalMilliseconds - _WaitAccuracy);
    }

    [Theory,
        InlineData(1),
        InlineData(2),
        InlineData(3)]
    internal static async Task Repeat_DelayOccurs(int tries)
    {
        Stopwatch watch = Stopwatch.StartNew();
        await new Limiter(tries, _SmallDelay).Repeat("", () => { }).ConfigureAwait(true);
        Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >=
            (_SmallDelay.TotalMilliseconds - _WaitAccuracy) * (tries - 1));
    }

    [Theory,
        InlineData(1),
        InlineData(2),
        InlineData(3)]
    internal static async Task StallUntil_DelayOccurs(int tries)
    {
        int attempts = 0;

        Stopwatch watch = Stopwatch.StartNew();
        await new Limiter(tries, _SmallDelay).StallUntil("", () => ++attempts == tries).ConfigureAwait(true);
        Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >=
            (_SmallDelay.TotalMilliseconds - _WaitAccuracy) * (tries - 1));
    }

    [Theory,
        InlineData(1),
        InlineData(2),
        InlineData(3)]
    internal static async Task Retry_DelayOccurs(int tries)
    {
        Exception exception = Tools.Randomizer.Create<Exception>();
        int attempts = 0;

        Stopwatch watch = Stopwatch.StartNew();
        await new Limiter(tries, _SmallDelay)
            .Retry("", () =>
            {
                if (++attempts != tries) { throw exception; }
            })
            .ConfigureAwait(true);
        Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >=
            (_SmallDelay.TotalMilliseconds - _WaitAccuracy) * (tries - 1));
    }

    [Theory,
        InlineData(1),
        InlineData(2),
        InlineData(3)]
    internal static async Task Attempt_DelayOccurs(int tries)
    {
        Exception exception = Tools.Randomizer.Create<Exception>();
        int attempts = 0;

        Stopwatch watch = Stopwatch.StartNew();
        await new Limiter(tries, _SmallDelay)
            .Attempt("", () =>
            {
                if (++attempts != tries) { throw exception; }
            })
            .ConfigureAwait(true);
        Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >=
            (_SmallDelay.TotalMilliseconds - _WaitAccuracy) * (tries - 1));
    }

    [Fact]
    internal static void Repeat_Cancelable()
    {
        using (CancellationTokenSource tokenSource = new())
        {
            _ = Tools.Asserter.Throws<TaskCanceledException>(
                () => Limiter.Few.Repeat("", () => tokenSource.Cancel(), tokenSource.Token).Wait());
        }

        _ = Tools.Asserter.Throws<TaskCanceledException>(
            () => Limiter.Few.Repeat("", () => { }, new CancellationToken(true)).Wait());

        _ = Tools.Asserter.Throws<TaskCanceledException>(
            () => Limiter.Quick.Repeat("", () => { }, new CancellationToken(true)).Wait());
    }

    [Fact]
    internal static void StallUntil_Cancelable()
    {
        using (CancellationTokenSource tokenSource = new())
        {
            _ = Tools.Asserter.Throws<TaskCanceledException>(
                () => Limiter.Few.StallUntil("", () => tokenSource.Cancel(), () => false, tokenSource.Token).Wait());
        }

        _ = Tools.Asserter.Throws<TaskCanceledException>(
            () => Limiter.Few.StallUntil("", () => false, new CancellationToken(true)).Wait());

        _ = Tools.Asserter.Throws<TaskCanceledException>(
            () => Limiter.Quick.StallUntil("", () => false, new CancellationToken(true)).Wait());
    }

    [Theory, RandomData]
    internal static void Retry_Cancelable(Exception exception)
    {
        using (CancellationTokenSource tokenSource = new())
        {
            _ = Tools.Asserter.Throws<TaskCanceledException>(() => Limiter.Few.Retry(
                "Message", () => throw exception, () => tokenSource.Cancel(), tokenSource.Token).Wait());
        }

        _ = Tools.Asserter.Throws<TaskCanceledException>(
            () => Limiter.Few.Retry("", () => throw exception, new CancellationToken(true)).Wait());

        _ = Tools.Asserter.Throws<TaskCanceledException>(
            () => Limiter.Quick.Retry("", () => throw exception, new CancellationToken(true)).Wait());
    }

    [Theory, RandomData]
    internal static void Attempt_Cancelable(Exception exception)
    {
        using (CancellationTokenSource tokenSource = new())
        {
            _ = Tools.Asserter.Throws<TaskCanceledException>(() => Limiter.Few.Attempt(
                "", () => throw exception, () => tokenSource.Cancel(), tokenSource.Token).Wait());
        }

        _ = Tools.Asserter.Throws<TaskCanceledException>(
            () => Limiter.Few.Attempt("", () => throw exception, new CancellationToken(true)).Wait());

        _ = Tools.Asserter.Throws<TaskCanceledException>(
            () => Limiter.Quick.Attempt("", () => throw exception, new CancellationToken(true)).Wait());
    }

    [Theory, RandomData]
    internal static async Task Repeat_ResultsValid(List<int> data)
    {
        int attempt = 0;

        Tools.Asserter.Is(data.AsReadOnly(), await new Limiter(data.Count)
            .Repeat("", () => data[attempt++])
            .ConfigureAwait(true));
    }

    [Theory, RandomData]
    internal static async Task StallUntil_ResultsValid(List<int> data)
    {
        int attempt = 0;

        Tools.Asserter.Is(data.AsReadOnly(), await new Limiter(data.Count)
            .StallUntil("", () => data[attempt++], () => attempt == data.Count)
            .ConfigureAwait(true));
    }

    [Theory, RandomData]
    internal static async Task Retry_ResultsValid(int data)
    {
        Tools.Asserter.Is(data, await new Limiter(1).Retry("", () => data).ConfigureAwait(true));
        Tools.Asserter.Is(data, await new Limiter(1).Retry("", () => data).ConfigureAwait(true));
    }

    [Fact]
    internal static async Task Retry_Continues()
    {
        int calls = 0;

        await new Limiter(2).Retry<ArithmeticException>("", () =>
        {
            calls++;
            if (calls == 1)
            {
                throw new ArithmeticException();
            }
        }).ConfigureAwait(true);
        Tools.Asserter.Is(2, calls);

        await new Limiter(2).Retry<SystemException>("", () =>
        {
            calls++;
            if (calls == 3)
            {
                throw new ArithmeticException();
            }
        }).ConfigureAwait(true);
        Tools.Asserter.Is(4, calls);
    }

    [Theory, RandomData]
    internal static async Task Attempt_ResultsValid(int data)
    {
        Tools.Asserter.Is(data, await new Limiter(1).Attempt("", () => data).ConfigureAwait(true));
        Tools.Asserter.Is(data, await new Limiter(1).Attempt("", () => data).ConfigureAwait(true));
    }

    [Fact]
    internal static async Task Attempt_Continues()
    {
        int calls = 0;

        await new Limiter(2).Attempt<ArithmeticException>("", () =>
        {
            calls++;
            if (calls == 1)
            {
                throw new ArithmeticException();
            }
        }).ConfigureAwait(true);
        Tools.Asserter.Is(2, calls);

        await new Limiter(2).Attempt<SystemException>("", () =>
        {
            calls++;
            if (calls == 3)
            {
                throw new ArithmeticException();
            }
        }).ConfigureAwait(true);
        Tools.Asserter.Is(4, calls);
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static async Task StallUntil_CheckStateBehavior(int tries)
    {
        int attempt = 0;
        int checkAttempt = 0;

        _ = await new Limiter(tries).StallUntil("", () => attempt++, () => ++checkAttempt == tries).ConfigureAwait(true);
        Tools.Asserter.Is(tries, attempt);
        Tools.Asserter.Is(tries, checkAttempt);
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static async Task Retry_ResetStateBehavior(int tries)
    {
        Exception exception = Tools.Randomizer.Create<Exception>();
        int attempt = 0;
        int resetAttempt = 0;

        await new Limiter(tries)
            .Retry("", () =>
            {
                if (++attempt != tries) { throw exception; }
            }, () => resetAttempt++)
            .ConfigureAwait(true);
        Tools.Asserter.Is(tries, attempt);
        Tools.Asserter.Is(tries - 1, resetAttempt);
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static async Task Retry_ReturnResetStateBehavior(int tries)
    {
        Exception exception = Tools.Randomizer.Create<Exception>();
        int attempt = 0;
        int resetAttempt = 0;

        int result = Tools.Randomizer.Create<int>();
        int ResetBehavior()
        {
            return (++attempt == tries) ? result : throw exception;
        }

        Tools.Asserter.Is(result, await new Limiter(tries)
            .Retry("", ResetBehavior, () => resetAttempt++)
            .ConfigureAwait(true));
        Tools.Asserter.Is(tries, attempt);
        Tools.Asserter.Is(tries - 1, resetAttempt);
    }

    [Theory, RandomData]
    internal static void Retry_WrongExceptionThrows(NotSupportedException exception)
    {
        Tools.Asserter.Is(exception, Tools.Asserter.Throws<NotSupportedException>(
            () => new Limiter(3).Retry<InvalidOperationException>("", (Action)(() => throw exception)).Wait()));

        IOException exception2 = new();

        Tools.Asserter.Is(exception2, Tools.Asserter.Throws<AggregateException>(
            () => new Limiter(3).Retry<DirectoryNotFoundException, bool>("",
                () => throw exception2).Wait()).InnerExceptions.Single());
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static async Task Attempt_ResetStateBehavior(int tries)
    {
        Exception exception = Tools.Randomizer.Create<Exception>();
        int attempt = 0;
        int resetAttempt = 0;

        await new Limiter(tries)
            .Attempt("", () =>
            {
                if (++attempt != tries) { throw exception; }
            }, () => resetAttempt++)
            .ConfigureAwait(true);
        Tools.Asserter.Is(tries, attempt);
        Tools.Asserter.Is(tries - 1, resetAttempt);
    }

    [Theory,
        InlineData(1),
        InlineData(3)]
    internal static async Task Attempt_ReturnResetStateBehavior(int tries)
    {
        Exception exception = Tools.Randomizer.Create<Exception>();
        int attempt = 0;
        int resetAttempt = 0;

        int result = Tools.Randomizer.Create<int>();
        int ResetBehavior()
        {
            return (++attempt == tries) ? result : throw exception;
        }

        Tools.Asserter.Is(result, await new Limiter(tries)
            .Attempt("", ResetBehavior, () => resetAttempt++)
            .ConfigureAwait(true));
        Tools.Asserter.Is(tries, attempt);
        Tools.Asserter.Is(tries - 1, resetAttempt);
    }

    [Theory, RandomData]
    internal static void Attempt_WrongExceptionThrows(NotSupportedException exception)
    {
        Tools.Asserter.Is(exception, Tools.Asserter.Throws<NotSupportedException>(
            () => new Limiter(3).Attempt<InvalidOperationException>(null, (Action)(() => throw exception)).Wait()));

        IOException exception2 = new();

        Tools.Asserter.Is(exception2, Tools.Asserter.Throws<AggregateException>(
            () => new Limiter(3).Attempt<DirectoryNotFoundException, bool>("",
                () => throw exception2).Wait()).InnerExceptions.Single());
    }

    [Fact]
    internal static void Limiter_DefaultsSet()
    {
        foreach (PropertyInfo info in typeof(Limiter)
            .GetProperties(BindingFlags.Static | BindingFlags.Public)
            .Where(p => p.PropertyType == typeof(Limiter)))
        {
            Tools.Asserter.IsNot(null, info.GetValue(null));
        }
    }

    [Theory, RandomData]
    internal static void Equality_MatchesValue(int tries, TimeSpan elapsed)
    {
        Limiter original = new(tries, elapsed);
        Limiter dupe = new(tries, elapsed);
        Limiter variant1 = new(Tools.Mutator.Variant(tries), elapsed);
        Limiter variant2 = new(tries, Tools.Mutator.Variant(elapsed));

        Tools.Asserter.Is(true, original.Equals(original));
        Tools.Asserter.Is(true, original.Equals(dupe));
        Tools.Asserter.Is(false, original.Equals(variant1));
        Tools.Asserter.Is(false, original.Equals(variant2));

        Tools.Asserter.Is(original.GetHashCode(), original.GetHashCode());
        Tools.Asserter.Is(original.GetHashCode(), dupe.GetHashCode());
        Tools.Asserter.IsNot(original.GetHashCode(), variant1.GetHashCode());
        Tools.Asserter.IsNot(original.GetHashCode(), variant2.GetHashCode());
    }

    [Theory, RandomData]
    internal static void ToString_Readable(int tries, TimeSpan timeout, TimeSpan delay)
    {
        Tools.Asserter.Is($"{tries}-{timeout}-{delay}",
            new Limiter(timeout, tries, delay).ToString());
    }
}

#pragma warning restore xUnit1031 // Test methods should not use blocking code
