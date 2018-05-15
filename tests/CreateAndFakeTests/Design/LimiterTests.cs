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

namespace CreateAndFakeTests.Design
{
    /// <summary>Verifies behavior.</summary>
    public static class LimiterTests
    {
        /// <summary>Small delay to test with.</summary>
        private static readonly TimeSpan s_SmallDelay = new TimeSpan(0, 0, 0, 0, 20);

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void Limiter_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(Limiter.Few);
        }

        /// <summary>Verifies limit starts after first attempt.</summary>
        [Fact]
        public static async Task Repeat_AtLeastOnce()
        {
            int attempts = 0;

            await new Limiter(0).Repeat(() => attempts++);
            Tools.Asserter.Is(1, attempts);

            await new Limiter(TimeSpan.MinValue).Repeat(() => attempts++);
            Tools.Asserter.Is(2, attempts);
        }

        /// <summary>Verifies limit starts after first attempt.</summary>
        [Fact]
        public static void StallUntil_AtLeastOnce()
        {
            int attempts = 0;

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(0).StallUntil(() => attempts++, () => false).Wait());
            Tools.Asserter.Is(1, attempts);

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(TimeSpan.MinValue).StallUntil(() => attempts++, () => false).Wait());
            Tools.Asserter.Is(2, attempts);
        }

        /// <summary>Verifies limit starts after first attempt.</summary>
        [Theory, RandomData]
        public static void Retry_AtLeastOnce(Exception exception)
        {
            int attempts = 0;

            Tools.Asserter.Is(exception, Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(0).Retry(() => { attempts++; throw exception; }).Wait()).InnerException);
            Tools.Asserter.Is(1, attempts);

            Tools.Asserter.Is(exception, Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(TimeSpan.MinValue).Retry(
                    () => { attempts++; throw exception; }).Wait()).InnerException);
            Tools.Asserter.Is(2, attempts);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [Theory,
            InlineData(1),
            InlineData(3)]
        public static async Task Repeat_TryLimited(int tries)
        {
            int attempts = 0;

            await new Limiter(tries).Repeat(() => attempts++);
            Tools.Asserter.Is(tries, attempts);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [Theory,
            InlineData(1),
            InlineData(3)]
        public static void StallUntil_TryLimited(int tries)
        {
            int attempts = 0;

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(tries).StallUntil(() => attempts++, () => false).Wait());
            Tools.Asserter.Is(tries, attempts);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [Theory,
            InlineData(1),
            InlineData(3)]
        public static void Retry_TryLimited(int tries)
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            int attempts = 0;

            Tools.Asserter.Is(exception, Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(tries).Retry(() => { attempts++; throw exception; }).Wait()).InnerException);
            Tools.Asserter.Is(tries, attempts);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [Fact]
        public static async Task Repeat_TimeoutLimited()
        {
            Stopwatch watch = Stopwatch.StartNew();
            await new Limiter(s_SmallDelay).Repeat(() => { });
            Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >= s_SmallDelay.TotalMilliseconds - 1);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [Fact]
        public static void StallUntil_TimeoutLimited()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(s_SmallDelay).StallUntil(() => { }, () => false).Wait());
            Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >= s_SmallDelay.TotalMilliseconds - 1);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [Theory, RandomData]
        public static void Retry_TimeoutLimited(Exception exception)
        {
            Stopwatch watch = Stopwatch.StartNew();

            Tools.Asserter.Is(exception, Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(s_SmallDelay).Retry(() => { throw exception; }).Wait()).InnerException);
            Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >= s_SmallDelay.TotalMilliseconds - 1);
        }

        /// <summary>Verifies limiter delays between attempts.</summary>
        [Theory,
            InlineData(1),
            InlineData(2),
            InlineData(3)]
        public static async Task Repeat_DelayOccurs(int tries)
        {
            Stopwatch watch = Stopwatch.StartNew();
            await new Limiter(tries, s_SmallDelay).Repeat(() => { });
            Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >=
                (s_SmallDelay.TotalMilliseconds - 1) * (tries - 1));
        }

        /// <summary>Verifies limiter delays between attempts.</summary>
        [Theory,
            InlineData(1),
            InlineData(2),
            InlineData(3)]
        public static async Task StallUntil_DelayOccurs(int tries)
        {
            int attempts = 0;

            Stopwatch watch = Stopwatch.StartNew();
            await new Limiter(tries, s_SmallDelay).StallUntil(() => ++attempts == tries);
            Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >=
                (s_SmallDelay.TotalMilliseconds - 1) * (tries - 1));
        }

        /// <summary>Verifies limiter delays between attempts.</summary>
        [Theory,
            InlineData(1),
            InlineData(2),
            InlineData(3)]
        public static async Task Retry_DelayOccurs(int tries)
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            int attempts = 0;

            Stopwatch watch = Stopwatch.StartNew();
            await new Limiter(tries, s_SmallDelay).Retry(() => { if (++attempts != tries) throw exception; });
            Tools.Asserter.Is(true, watch.Elapsed.TotalMilliseconds >=
                (s_SmallDelay.TotalMilliseconds - 1) * (tries - 1));
        }

        /// <summary>Verifies the task can be canceled.</summary>
        [Fact]
        public static void Repeat_Cancelable()
        {
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                Tools.Asserter.Throws<TaskCanceledException>(
                    () => Limiter.Few.Repeat(() => tokenSource.Cancel(), tokenSource.Token).Wait());
            }

            Tools.Asserter.Throws<TaskCanceledException>(
                () => Limiter.Few.Repeat(() => { }, new CancellationToken(true)).Wait());

            Tools.Asserter.Throws<TaskCanceledException>(
                () => Limiter.Quick.Repeat(() => { }, new CancellationToken(true)).Wait());
        }

        /// <summary>Verifies the task can be canceled.</summary>
        [Fact]
        public static void StallUntil_Cancelable()
        {
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                Tools.Asserter.Throws<TaskCanceledException>(
                    () => Limiter.Few.StallUntil(() => tokenSource.Cancel(), () => false, tokenSource.Token).Wait());
            }

            Tools.Asserter.Throws<TaskCanceledException>(
                () => Limiter.Few.StallUntil(() => false, new CancellationToken(true)).Wait());

            Tools.Asserter.Throws<TaskCanceledException>(
                () => Limiter.Quick.StallUntil(() => false, new CancellationToken(true)).Wait());
        }

        /// <summary>Verifies the task can be canceled.</summary>
        [Theory, RandomData]
        public static void Retry_Cancelable(Exception exception)
        {
            using (CancellationTokenSource tokenSource = new CancellationTokenSource())
            {
                Tools.Asserter.Throws<TaskCanceledException>(() => Limiter.Few.Retry(
                    () => throw exception, () => tokenSource.Cancel(), tokenSource.Token).Wait());
            }

            Tools.Asserter.Throws<TaskCanceledException>(
                () => Limiter.Few.Retry(() => throw exception, new CancellationToken(true)).Wait());

            Tools.Asserter.Throws<TaskCanceledException>(
                () => Limiter.Quick.Retry(() => throw exception, new CancellationToken(true)).Wait());
        }

        /// <summary>Verifies limiter collates results correctly.</summary>
        [Theory, RandomData]
        public static async Task Repeat_ResultsValid(List<int> data)
        {
            int attempt = 0;

            Tools.Asserter.Is(data.AsReadOnly(),
                await new Limiter(data.Count).Repeat(() => data[attempt++]));
        }

        /// <summary>Verifies limiter collates results correctly.</summary>
        [Theory, RandomData]
        public static async Task StallUntil_ResultsValid(List<int> data)
        {
            int attempt = 0;

            Tools.Asserter.Is(data.AsReadOnly(),
                await new Limiter(data.Count).StallUntil(() => data[attempt++], () => attempt == data.Count));
        }

        /// <summary>Verifies limiter collates results correctly.</summary>
        [Theory, RandomData]
        public static async Task Retry_ResultsValid(int data)
        {
            Tools.Asserter.Is(data, await new Limiter(1).Retry(() => data));
            Tools.Asserter.Is(data, await new Limiter(1).Retry(() => data));
        }

        /// <summary>Verifies limiter retries until success.</summary>
        [Fact]
        public static async Task Retry_Continues()
        {
            int calls = 0;

            await new Limiter(2).Retry<ArithmeticException>(() =>
            {
                calls++;
                if (calls == 1)
                {
                    throw new ArithmeticException();
                }
            });
            Tools.Asserter.Is(2, calls);

            await new Limiter(2).Retry<SystemException>(() =>
            {
                calls++;
                if (calls == 3)
                {
                    throw new ArithmeticException();
                }
            });
            Tools.Asserter.Is(4, calls);
        }

        /// <summary>Verifies that check state is called properly.</summary>
        [Theory,
            InlineData(1),
            InlineData(3)]
        public static async Task StallUntil_CheckStateBehavior(int tries)
        {
            int attempt = 0;
            int checkAttempt = 0;

            await new Limiter(tries).StallUntil(() => attempt++, () => ++checkAttempt == tries);
            Tools.Asserter.Is(tries, attempt);
            Tools.Asserter.Is(tries, checkAttempt);
        }

        /// <summary>Verifies that reset state is called properly.</summary>
        [Theory,
            InlineData(1),
            InlineData(3)]
        public static async Task Retry_ResetStateBehavior(int tries)
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            int attempt = 0;
            int resetAttempt = 0;

            await new Limiter(tries).Retry(() => { if (++attempt != tries) throw exception; }, () => resetAttempt++);
            Tools.Asserter.Is(tries, attempt);
            Tools.Asserter.Is(tries - 1, resetAttempt);
        }

        /// <summary>Verifies that reset state is called properly.</summary>
        [Theory,
            InlineData(1),
            InlineData(3)]
        public static async Task Retry_ReturnResetStateBehavior(int tries)
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            int attempt = 0;
            int resetAttempt = 0;

            int result = Tools.Randomizer.Create<int>();
            int ResetBehavior()
            {
                return (++attempt == tries) ? result : throw exception;
            };

            Tools.Asserter.Is(result, await new Limiter(tries).Retry(ResetBehavior, () => resetAttempt++));
            Tools.Asserter.Is(tries, attempt);
            Tools.Asserter.Is(tries - 1, resetAttempt);
        }

        /// <summary>Verifies retry only catches expected exceptions.</summary>
        [Theory, RandomData]
        public static void Retry_WrongExceptionThrows(NotSupportedException exception)
        {
            Tools.Asserter.Is(exception, Tools.Asserter.Throws<NotSupportedException>(
                () => new Limiter(3).Retry<InvalidOperationException>((Action)(() => throw exception)).Wait()));

            IOException exception2 = new IOException();

            Tools.Asserter.Is(exception2, Tools.Asserter.Throws<AggregateException>(
                () => new Limiter(3).Retry<DirectoryNotFoundException, bool>(
                    () => throw exception2).Wait()).InnerExceptions.Single());
        }

        /// <summary>Verifies default instances are not null.</summary>
        [Fact]
        public static void Limiter_DefaultsSet()
        {
            foreach (PropertyInfo info in typeof(Limiter)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(Limiter)))
            {
                Tools.Asserter.IsNot(null, info.GetValue(null));
            }
        }

        /// <summary>Verifies the equality methods work properly.</summary>
        [Theory, RandomData(Trials = 3)]
        public static void Equality_MatchesValue(int tries, TimeSpan elapsed)
        {
            Limiter original = new Limiter(tries, elapsed);
            Limiter dupe = new Limiter(tries, elapsed);
            Limiter variant1 = new Limiter(Tools.Mutator.Variant(tries), elapsed);
            Limiter variant2 = new Limiter(tries, Tools.Mutator.Variant(elapsed));

            Tools.Asserter.Is(true, original.Equals(original));
            Tools.Asserter.Is(false, original.Equals(null));
            Tools.Asserter.Is(true, original.Equals(dupe));
            Tools.Asserter.Is(false, original.Equals(variant1));
            Tools.Asserter.Is(false, original.Equals(variant2));

            Tools.Asserter.Is(original.GetHashCode(), original.GetHashCode());
            Tools.Asserter.Is(original.GetHashCode(), dupe.GetHashCode());
            Tools.Asserter.IsNot(original.GetHashCode(), variant1.GetHashCode());
            Tools.Asserter.IsNot(original.GetHashCode(), variant2.GetHashCode());
        }

        /// <summary>Verifies readable as string.</summary>
        [Theory, RandomData]
        public static void ToString_Readable(int tries, TimeSpan timeout, TimeSpan delay)
        {
            Tools.Asserter.Is($"{tries}-{timeout}-{delay}",
                new Limiter(timeout, tries, delay).ToString());
        }
    }
}
