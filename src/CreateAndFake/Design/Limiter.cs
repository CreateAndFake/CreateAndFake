using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Design
{
    /// <summary>Provides the core functionality for repetition.</summary>
    public sealed class Limiter : IEquatable<Limiter>
    {
        /// <summary>Instance that defaults to a single attempt.</summary>
        public static Limiter Once { get; } = new Limiter(1);

        /// <summary>Instance that default to five attempts.</summary>
        public static Limiter Few { get; } = new Limiter(5);

        /// <summary>Instance that defaults to a dozen attempts.</summary>
        public static Limiter Dozen { get; } = new Limiter(12);

        /// <summary>Instance that defaults to a thousand attempts.</summary>
        public static Limiter Myriad { get; } = new Limiter(1000);

        /// <summary>Instance that defaults to half a second with a small delay.</summary>
        public static Limiter Quick { get; } = new Limiter(
            new TimeSpan(0, 0, 0, 0, 500), new TimeSpan(0, 0, 0, 0, 20));

        /// <summary>Instance that defaults to five seconds with a large delay.</summary>
        public static Limiter Slow { get; } = new Limiter(
            new TimeSpan(0, 0, 5), new TimeSpan(0, 0, 0, 0, 200));

        /// <summary>Maximum attempts to try.</summary>
        private readonly int _tries;

        /// <summary>Maximum length to attempt for.</summary>
        private readonly TimeSpan _timeout;

        /// <summary>Delay between attempts.</summary>
        private readonly TimeSpan _delay;

        /// <summary>Continues until the given number of attempts.</summary>
        /// <param name="tries">Maximum attempts to try.</param>
        /// <param name="delay">Delay between attempts.</param>
        public Limiter(int tries, TimeSpan? delay = null)
            : this(TimeSpan.MaxValue, tries, delay) { }

        /// <summary>Continues until the given length of time.</summary>
        /// <param name="timeout">Maximum length to attempt for.</param>
        /// <param name="delay">Delay between attempts.</param>
        public Limiter(TimeSpan timeout, TimeSpan? delay = null)
            : this(timeout, int.MaxValue, delay) { }

        /// <summary>Continues until a number of attempts or timeout is reached.</summary>
        /// <param name="timeout">Maximum length to attempt for.</param>
        /// <param name="tries">Maximum attempts to try.</param>
        /// <param name="delay">Delay between attempts.</param>
        public Limiter(TimeSpan timeout, int tries, TimeSpan? delay = null)
        {
            _tries = tries;
            _timeout = timeout;
            _delay = delay ?? TimeSpan.Zero;
        }

        /// <summary>Repeats an action.</summary>
        /// <param name="action">Action to repeatably do.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Awaitable task.</returns>
        public async Task Repeat(Action action, CancellationToken? canceler = null)
        {
            await Repeat(() => { action?.Invoke(); return true; }, canceler).ConfigureAwait(false);
        }

        /// <summary>Repeats an action.</summary>
        /// <typeparam name="T">Result type returned from the action.</typeparam>
        /// <param name="action">Action to repeatably do.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Results from all calls.</returns>
        public async Task<IReadOnlyCollection<T>> Repeat<T>(Func<T> action, CancellationToken? canceler = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            List<T> results = new List<T>();

            Stopwatch watch = Stopwatch.StartNew();
            int attempt = 0;
            do
            {
                results.Add(action.Invoke());
            } while (await DelayIfNotDone(watch.Elapsed, ++attempt, canceler).ConfigureAwait(false));

            return results.AsReadOnly();
        }

        /// <summary>Retries an action until it returns true.</summary>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task StallUntil(Func<bool> action, CancellationToken? canceler = null)
        {
            await StallUntil(null, action, canceler).ConfigureAwait(false);
        }

        /// <summary>Retries an action until a given state.</summary>
        /// <param name="action">Action to attempt.</param>
        /// <param name="checkState">Polls if the action was successful.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task StallUntil(Action action, Func<bool> checkState, CancellationToken? canceler = null)
        {
            await StallUntil(() => { action?.Invoke(); return true; }, checkState, canceler).ConfigureAwait(false);
        }

        /// <summary>Retries an action until a given state.</summary>
        /// <typeparam name="T">Result type returned from the action.</typeparam>
        /// <param name="action">Action to attempt.</param>
        /// <param name="checkState">Polls if the action was successful.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Results from all calls.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task<IReadOnlyCollection<T>> StallUntil<T>(
            Func<T> action, Func<bool> checkState, CancellationToken? canceler = null)
        {
            if (action == null) throw new ArgumentNullException(nameof(checkState));
            if (checkState == null) throw new ArgumentNullException(nameof(checkState));

            List<T> results = new List<T>();

            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 1; true; i++)
            {
                results.Add(action.Invoke());
                if (checkState.Invoke())
                {
                    break;
                }
                else
                {
                    await DelayOrFault(watch.Elapsed, i, canceler).ConfigureAwait(false);
                }
            }

            return results.AsReadOnly();
        }

        /// <summary>Retries an action when encountering exceptions.</summary>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task Retry(Action action, CancellationToken? canceler = null)
        {
            await Retry<Exception>(action, null, canceler).ConfigureAwait(false);
        }

        /// <summary>Retries an action when encountering exceptions.</summary>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="resetState">Action to reset state before next attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task Retry(Action action, Action resetState, CancellationToken? canceler = null)
        {
            await Retry<Exception>(action, resetState, canceler).ConfigureAwait(false);
        }

        /// <summary>Retries an action when encountering exceptions.</summary>
        /// <typeparam name="TException">Exception type to ignore and retry if encountered.</typeparam>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task Retry<TException>(Action action, CancellationToken? canceler = null)
            where TException : Exception
        {
            await Retry<TException>(action, null, canceler).ConfigureAwait(false);
        }

        /// <summary>Retries an action when encountering exceptions.</summary>
        /// <typeparam name="TException">Exception type to ignore and retry if encountered.</typeparam>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="resetState">Action to reset state before next attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task Retry<TException>(Action action, Action resetState, CancellationToken? canceler = null)
            where TException : Exception
        {
            await Retry<TException, bool>(
                () => { action?.Invoke(); return true; }, resetState, canceler)
                .ConfigureAwait(false);
        }

        /// <summary>Retries an action when encountering exceptions.</summary>
        /// <typeparam name="TResult">Result type returned from the action.</typeparam>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Result of the successful attempt.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task<TResult> Retry<TResult>(Func<TResult> action, CancellationToken? canceler = null)
        {
            return await Retry<Exception, TResult>(action, null, canceler).ConfigureAwait(false);
        }

        /// <summary>Retries an action when encountering exceptions.</summary>
        /// <typeparam name="TResult">Result type returned from the action.</typeparam>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="resetState">Action to reset state before next attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Result of the successful attempt.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task<TResult> Retry<TResult>(
            Func<TResult> action, Action resetState, CancellationToken? canceler = null)
        {
            return await Retry<Exception, TResult>(action, resetState, canceler).ConfigureAwait(false);
        }

        /// <summary>Retries an action when encountering exceptions.</summary>
        /// <typeparam name="TException">Exception type to ignore and retry if encountered.</typeparam>
        /// <typeparam name="TResult">Result type returned from the action.</typeparam>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Result of the successful attempt.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task<TResult> Retry<TException, TResult>(Func<TResult> action, CancellationToken? canceler = null)
            where TException : Exception
        {
            return await Retry<TException, TResult>(action, null, canceler).ConfigureAwait(false);
        }

        /// <summary>Retries an action when encountering exceptions.</summary>
        /// <typeparam name="TException">Exception type to ignore and retry if encountered.</typeparam>
        /// <typeparam name="TResult">Result type returned from the action.</typeparam>
        /// <param name="action">Action to repeatably attempt.</param>
        /// <param name="resetState">Action to reset state before next attempt.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Result of the successful attempt.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        public async Task<TResult> Retry<TException, TResult>(
            Func<TResult> action, Action resetState, CancellationToken? canceler = null)
            where TException : Exception
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 1; true; i++)
            {
                try
                {
                    return action.Invoke();
                }
                catch (TException error)
                {
                    resetState?.Invoke();
                    await DelayOrFault(watch.Elapsed, i, canceler, error).ConfigureAwait(false);
                }
            }
        }

        /// <summary>Delays if terminal condition not reached.</summary>
        /// <param name="elapsed">Current amount of time that has elapsed.</param>
        /// <param name="tries">Current number of attempts.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>True if terminal condition not reached; false otherwise.</returns>
        private async Task<bool> DelayIfNotDone(TimeSpan elapsed, int tries, CancellationToken? canceler)
        {
            if (tries < _tries && elapsed < _timeout)
            {
                await DelayOrCancel(canceler).ConfigureAwait(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Faults if terminal condition reached; delays otherwise.</summary>
        /// <param name="elapsed">Current amount of time that has elapsed.</param>
        /// <param name="tries">Current number of attempts.</param>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <param name="ex">Current exception if present.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
        private async Task DelayOrFault(TimeSpan elapsed, int tries, CancellationToken? canceler, Exception ex = null)
        {
            if (tries >= _tries)
            {
                throw new TimeoutException($"Reached max attempts of '{_tries}'.", ex);
            }
            else if (elapsed >= _timeout)
            {
                throw new TimeoutException($"Reached timeout of '{_timeout}'.", ex);
            }
            else
            {
                await DelayOrCancel(canceler).ConfigureAwait(false);
            }
        }

        /// <summary>Faults if task has been canceled; delays otherwise.</summary>
        /// <param name="canceler">Token to potentially cancel the attempts.</param>
        /// <returns>Awaitable task.</returns>
        private async Task DelayOrCancel(CancellationToken? canceler)
        {
            CancellationToken token = canceler ?? CancellationToken.None;
            if (_delay > TimeSpan.Zero)
            {
                await Task.Delay(_delay, token).ConfigureAwait(false);
            }
            else
            {
                token.ThrowIfCancellationRequested();
            }
        }

        /// <summary>Checks for value equality.</summary>
        /// <param name="obj">Instance to compare with.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Limiter);
        }

        /// <summary>Checks for value equality.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public bool Equals(Limiter other)
        {
            return other != null
                && _delay == other._delay
                && _timeout == other._timeout
                && _tries == other._tries;
        }

        /// <returns>Hash code based upon value identifying the object.</returns>
        public override int GetHashCode()
        {
            return ValueComparer.Use.GetHashCode(_tries, _timeout, _delay);
        }

        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            return $"{_tries}-{_timeout}-{_delay}";
        }
    }
}
