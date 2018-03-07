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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Design
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class LimiterTests
    {
        /// <summary>Small delay to test with.</summary>
        private static readonly TimeSpan s_SmallDelay = new TimeSpan(0, 0, 0, 0, 20);

        /// <summary>Verifies limit starts after first attempt.</summary>
        [TestMethod]
        public async Task Repeat_AtLeastOnce()
        {
            int attempts = 0;

            await new Limiter(0).Repeat(() => attempts++);
            Tools.Asserter.Is(1, attempts);

            await new Limiter(TimeSpan.MinValue).Repeat(() => attempts++);
            Tools.Asserter.Is(2, attempts);
        }

        /// <summary>Verifies limit starts after first attempt.</summary>
        [TestMethod]
        public void StallUntil_AtLeastOnce()
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
        [TestMethod]
        public void Retry_AtLeastOnce()
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            int attempts = 0;

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(0).Retry(() => { attempts++; throw exception; }).Wait(),
                e => Tools.Asserter.Is(exception, e.InnerException));
            Tools.Asserter.Is(1, attempts);

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(TimeSpan.MinValue).Retry(() => { attempts++; throw exception; }).Wait(),
                e => Tools.Asserter.Is(exception, e.InnerException));
            Tools.Asserter.Is(2, attempts);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [TestMethod]
        public async Task Repeat_TryLimited()
        {
            int attempts = 0;

            await new Limiter(1).Repeat(() => attempts++);
            Tools.Asserter.Is(1, attempts);

            await new Limiter(3).Repeat(() => attempts++);
            Tools.Asserter.Is(4, attempts);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [TestMethod]
        public void StallUntil_TryLimited()
        {
            int attempts = 0;

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(1).StallUntil(() => attempts++, () => false).Wait());
            Tools.Asserter.Is(1, attempts);

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(3).StallUntil(() => attempts++, () => false).Wait());
            Tools.Asserter.Is(4, attempts);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [TestMethod]
        public void Retry_TryLimited()
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            int attempts = 0;

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(1).Retry(() => { attempts++; throw exception; }).Wait(),
                e => Tools.Asserter.Is(exception, e.InnerException));
            Tools.Asserter.Is(1, attempts);

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(3).Retry(() => { attempts++; throw exception; }).Wait(),
                e => Tools.Asserter.Is(exception, e.InnerException));
            Tools.Asserter.Is(4, attempts);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [TestMethod]
        public async Task Repeat_TimeoutLimited()
        {
            Stopwatch watch = Stopwatch.StartNew();
            await new Limiter(s_SmallDelay).Repeat(() => { });
            Tools.Asserter.Is(true, watch.Elapsed.Ticks < s_SmallDelay.Ticks * 2);

            watch.Restart();
            await new Limiter(s_SmallDelay).Repeat(() => { });
            Tools.Asserter.Is(true, watch.Elapsed.Ticks < s_SmallDelay.Ticks * 3);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [TestMethod]
        public void StallUntil_TimeoutLimited()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(s_SmallDelay).StallUntil(() => { }, () => false).Wait());
            Tools.Asserter.Is(true, watch.Elapsed.Ticks < s_SmallDelay.Ticks * 2);

            watch.Restart();
            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(s_SmallDelay).StallUntil(() => { }, () => false).Wait());
            Tools.Asserter.Is(true, watch.Elapsed.Ticks < s_SmallDelay.Ticks * 3);
        }

        /// <summary>Verifies limiter actually limits.</summary>
        [TestMethod]
        public void Retry_TimeoutLimited()
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            Stopwatch watch = Stopwatch.StartNew();

            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(s_SmallDelay).Retry(() => { throw exception; }).Wait(),
                e => Tools.Asserter.Is(exception, e.InnerException));
            Tools.Asserter.Is(true, watch.Elapsed.Ticks < s_SmallDelay.Ticks * 2);

            watch.Restart();
            Tools.Asserter.Throws<TimeoutException>(
                () => new Limiter(s_SmallDelay).Retry(() => { throw exception; }).Wait(),
                e => Tools.Asserter.Is(exception, e.InnerException));
            Tools.Asserter.Is(true, watch.Elapsed.Ticks < s_SmallDelay.Ticks * 3);
        }

        /// <summary>Verifies limiter delays between attempts.</summary>
        [TestMethod]
        public async Task Repeat_DelayOccurs()
        {
            Stopwatch watch = Stopwatch.StartNew();
            await new Limiter(2, s_SmallDelay).Repeat(() => { });
            Tools.Asserter.Is(true, watch.Elapsed.Ticks >= s_SmallDelay.Ticks / 2);
        }

        /// <summary>Verifies limiter delays between attempts.</summary>
        [TestMethod]
        public async Task StallUntil_DelayOccurs()
        {
            int attempts = 0;

            Stopwatch watch = Stopwatch.StartNew();
            await new Limiter(2, s_SmallDelay).StallUntil(() => ++attempts == 2);
            Tools.Asserter.Is(true, watch.Elapsed.Ticks >= s_SmallDelay.Ticks / 2);
        }

        /// <summary>Verifies limiter delays between attempts.</summary>
        [TestMethod]
        public async Task Retry_DelayOccurs()
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            int attempts = 0;

            Stopwatch watch = Stopwatch.StartNew();
            await new Limiter(2, s_SmallDelay).Retry(() => { if (++attempts != 2) throw exception; });
            Tools.Asserter.Is(true, watch.Elapsed.Ticks >= s_SmallDelay.Ticks / 2);
        }

        /// <summary>Verifies the task can be canceled.</summary>
        [TestMethod]
        public void Repeat_Cancelable()
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
        [TestMethod]
        public void StallUntil_Cancelable()
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
        [TestMethod]
        public void Retry_Cancelable()
        {
            Exception exception = Tools.Randomizer.Create<Exception>();

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
        [TestMethod]
        public async Task Repeat_ResultsValid()
        {
            List<int> data = Tools.Randomizer.Create<List<int>>();
            int attempt = 0;

            Tools.Asserter.Is(data.AsReadOnly(),
                await new Limiter(data.Count).Repeat(() => data[attempt++]));
        }

        /// <summary>Verifies limiter collates results correctly.</summary>
        [TestMethod]
        public async Task StallUntil_ResultsValid()
        {
            List<int> data = Tools.Randomizer.Create<List<int>>();
            int attempt = 0;

            Tools.Asserter.Is(data.AsReadOnly(),
                await new Limiter(data.Count).StallUntil(() => data[attempt++], () => attempt == data.Count));
        }

        /// <summary>Verifies limiter collates results correctly.</summary>
        [TestMethod]
        public async Task Retry_ResultsValid()
        {
            int data = Tools.Randomizer.Create<int>();

            Tools.Asserter.Is(data, await new Limiter(1).Retry(() => data));
            Tools.Asserter.Is(data, await new Limiter(1).Retry(() => data));
        }

        /// <summary>Verifies limiter retries until success.</summary>
        [TestMethod]
        public async Task Retry_Continues()
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

        /// <summary>Verifies null works as intended.</summary>
        [TestMethod]
        public async Task Repeat_NullBehavior()
        {
            await Limiter.Few.Repeat(null);

            Tools.Asserter.Throws<ArgumentNullException>(
                () => Limiter.Few.Repeat<object>(null).Wait());
        }

        /// <summary>Verifies null works as intended.</summary>
        [TestMethod]
        public async Task StallUntil_NullBehavior()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => Limiter.Few.StallUntil(null).Wait());

            await Limiter.Few.StallUntil(null, () => true);

            Tools.Asserter.Throws<ArgumentNullException>(
                () => Limiter.Few.StallUntil(() => { }, null).Wait());

            Tools.Asserter.Throws<ArgumentNullException>(
                () => Limiter.Few.StallUntil<object>(null, () => true).Wait());
        }

        /// <summary>Verifies null works as intended.</summary>
        [TestMethod]
        public async Task Retry_NullBehavior()
        {
            await Limiter.Few.Retry(null);
            await Limiter.Few.Retry(null, null);

            Tools.Asserter.Throws<ArgumentNullException>(
                () => Limiter.Few.Retry<bool>(null).Wait());

            await Limiter.Few.Retry(() => true, null);
        }

        /// <summary>Verifies that check state is called properly.</summary>
        [TestMethod]
        public async Task StallUntil_CheckStateBehavior()
        {
            int attempt = 0;
            int checkAttempt = 0;

            await new Limiter(1).StallUntil(() => attempt++, () => ++checkAttempt == 1);
            Tools.Asserter.Is(1, attempt);
            Tools.Asserter.Is(1, checkAttempt);

            await new Limiter(3).StallUntil(() => attempt++, () => ++checkAttempt == 4);
            Tools.Asserter.Is(4, attempt);
            Tools.Asserter.Is(4, checkAttempt);
        }

        /// <summary>Verifies that reset state is called properly.</summary>
        [TestMethod]
        public async Task Retry_ResetStateBehavior()
        {
            Exception exception = Tools.Randomizer.Create<Exception>();
            int attempt = 0;
            int resetAttempt = 0;

            await new Limiter(1).Retry(() => attempt++, () => resetAttempt++);
            Tools.Asserter.Is(1, attempt);
            Tools.Asserter.Is(0, resetAttempt);

            await new Limiter(3).Retry(() => { if (++attempt != 4) throw exception; }, () => resetAttempt++);
            Tools.Asserter.Is(4, attempt);
            Tools.Asserter.Is(2, resetAttempt);
        }

        /// <summary>Verifies retry only catches expected exceptions.</summary>
        [TestMethod]
        public void Retry_WrongExceptionThrows()
        {
            NotSupportedException exception = Tools.Randomizer.Create<NotSupportedException>();

            Tools.Asserter.Throws<NotSupportedException>(
                () => new Limiter(3).Retry<InvalidOperationException>((Action)(() => throw exception)).Wait(),
                e => Tools.Asserter.Is(exception, e));

            IOException exception2 = new IOException();

            Tools.Asserter.Throws<AggregateException>(
                () => new Limiter(3).Retry<DirectoryNotFoundException, bool>(() => throw exception2).Wait(),
                e => Tools.Asserter.Is(exception2, e.InnerExceptions.Single()));
        }

        /// <summary>Verifies default instances are not null.</summary>
        [TestMethod]
        public void Limiter_DefaultsSet()
        {
            foreach (PropertyInfo info in typeof(Limiter)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(Limiter)))
            {
                Tools.Asserter.IsNot(null, info.GetValue(null));
            }
        }

        /// <summary>Verifies the equality methods work properly.</summary>
        [TestMethod]
        public void Equality_MatchesValue()
        {
            int tries = Tools.Randomizer.Create<int>();
            TimeSpan elapsed = Tools.Randomizer.Create<TimeSpan>();

            Limiter original = new Limiter(tries, elapsed);
            Limiter dupe = new Limiter(tries, elapsed);
            Limiter variant1 = new Limiter(Tools.Randiffer.Branch(tries), elapsed);
            Limiter variant2 = new Limiter(tries, Tools.Randiffer.Branch(elapsed));

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
        [TestMethod]
        public void ToString_Readable()
        {
            int tries = Tools.Randomizer.Create<int>();
            TimeSpan timeout = Tools.Randomizer.Create<TimeSpan>();
            TimeSpan delay = Tools.Randomizer.Create<TimeSpan>();

            Tools.Asserter.Is(tries + "-" + timeout + "-" + delay,
                new Limiter(timeout, tries, delay).ToString());
        }
    }
}
