using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Design;

/// <summary>Provides the core functionality for repetition.</summary>
/// <param name="timeout">Maximum length to attempt for.</param>
/// <param name="tries">Maximum attempts to try.</param>
/// <param name="delay">Delay between attempts.</param>
public sealed class Limiter(TimeSpan timeout, int tries, TimeSpan? delay = null) : IEquatable<Limiter>
{
    /// <summary>Instance that defaults to a single attempt.</summary>
    public static Limiter Once { get; } = new Limiter(1);

    /// <summary>Instance that default to five attempts.</summary>
    public static Limiter Few { get; } = new Limiter(5);

    /// <summary>Instance that defaults to a dozen attempts.</summary>
    public static Limiter Dozen { get; } = new Limiter(12);

    /// <summary>Instance that defaults to a hundred attempts.</summary>
    public static Limiter Hundred { get; } = new Limiter(100);

    /// <summary>Instance that defaults to a thousand attempts.</summary>
    public static Limiter Myriad { get; } = new Limiter(1000);

    /// <summary>Instance that defaults to half a second with a small delay.</summary>
    public static Limiter Quick { get; } = new Limiter(
        new TimeSpan(0, 0, 0, 0, 500), new TimeSpan(0, 0, 0, 0, 20));

    /// <summary>Instance that defaults to five seconds with a large delay.</summary>
    public static Limiter Slow { get; } = new Limiter(
        new TimeSpan(0, 0, 5), new TimeSpan(0, 0, 0, 0, 200));

    /// <summary>Maximum attempts to try.</summary>
    private readonly int _tries = tries;

    /// <summary>Maximum duration to attempt for.</summary>
    private readonly TimeSpan _timeout = timeout;

    /// <summary>Delay between attempts.</summary>
    private readonly TimeSpan _delay = delay ?? TimeSpan.Zero;

    /// <inheritdoc cref="Limiter(TimeSpan,int,TimeSpan?)"/>
    public Limiter(int tries, TimeSpan? delay = null)
        : this(TimeSpan.MaxValue, tries, delay) { }

    /// <inheritdoc cref="Limiter(TimeSpan,int,TimeSpan?)"/>
    public Limiter(TimeSpan timeout, TimeSpan? delay = null)
        : this(timeout, int.MaxValue, delay) { }

    /// <returns>Awaitable task handling the repetitions.</returns>
    /// <inheritdoc cref="Repeat{T}(string,Func{T},CancellationToken?)"/>
    public Task Repeat(string message, Action behavior, CancellationToken? canceler = null)
    {
        return Repeat(message, () => { behavior?.Invoke(); return true; }, canceler);
    }

    /// <summary>Repeats <paramref name="behavior"/>.</summary>
    /// <typeparam name="T">Result type returned from the action.</typeparam>
    /// <param name="message">Details to include upon a timeout exception.</param>
    /// <param name="behavior">Behavior to repeatably do.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>Results from all calls.</returns>
    public async Task<IReadOnlyCollection<T>> Repeat<T>(
        string message, Func<T> behavior, CancellationToken? canceler = null)
    {
        ArgumentGuard.ThrowIfNull(behavior, nameof(behavior));

        List<T> results = [];

        Stopwatch watch = Stopwatch.StartNew();
        int attempt = 0;
        do
        {
            results.Add(behavior.Invoke());
        } while (await DelayIfNotDone(message, watch.Elapsed, ++attempt, canceler).ConfigureAwait(false));

        return results.AsReadOnly();
    }

    /// <summary>Retries <paramref name="behavior"/> until it returns true.</summary>
    /// <inheritdoc cref="StallUntil(string,Action,Func{bool},CancellationToken?)"/>
    public Task StallUntil(string message, Func<bool> behavior, CancellationToken? canceler = null)
    {
        return StallUntil(message, null, behavior, canceler);
    }

    /// <summary>Retries <paramref name="behavior"/> until <paramref name="checkState"/>.</summary>
    /// <returns>Awaitable task handling the repetitions.</returns>
    /// <inheritdoc cref="StallUntil{T}(string,Func{T},Func{bool},CancellationToken?)"/>
    public Task StallUntil(string message, Action behavior, Func<bool> checkState, CancellationToken? canceler = null)
    {
        return StallUntil(message, () => { behavior?.Invoke(); return true; }, checkState, canceler);
    }

    /// <summary>Retries <paramref name="behavior"/> until <paramref name="checkState"/>.</summary>
    /// <typeparam name="T">Result type returned from the action.</typeparam>
    /// <param name="message">Details to include upon a timeout exception.</param>
    /// <param name="behavior">Behavior to repeatably attempt.</param>
    /// <param name="checkState">Polls if the behavior was successful.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>Results from all calls.</returns>
    /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
    /// <remarks>Beware infinite loops in <paramref name="behavior"/>.</remarks>
    public async Task<IReadOnlyCollection<T>> StallUntil<T>(
        string message, Func<T> behavior, Func<bool> checkState, CancellationToken? canceler = null)
    {
        ArgumentGuard.ThrowIfNull(behavior, nameof(behavior));
        ArgumentGuard.ThrowIfNull(checkState, nameof(checkState));

        List<T> results = [];

        Stopwatch watch = Stopwatch.StartNew();
        for (int i = 1; true; i++)
        {
            results.Add(behavior.Invoke());
            if (checkState.Invoke())
            {
                break;
            }
            else
            {
                await DelayOrFault(message, watch.Elapsed, i, canceler).ConfigureAwait(false);
            }
        }

        return results.AsReadOnly();
    }

    /// <inheritdoc cref="Retry{T}(string,Action,Action,CancellationToken?)"/>
    public Task Retry(string message, Action behavior, CancellationToken? canceler = null)
    {
        return Retry<Exception>(message, behavior, null, canceler);
    }

    /// <inheritdoc cref="Retry{T}(string,Action,Action,CancellationToken?)"/>
    public Task Retry(string message, Action behavior, Action resetState, CancellationToken? canceler = null)
    {
        return Retry<Exception>(message, behavior, resetState, canceler);
    }

    /// <inheritdoc cref="Retry{T}(string,Action,Action,CancellationToken?)"/>
    public Task Retry<TException>(string message, Action behavior, CancellationToken? canceler = null)
        where TException : Exception
    {
        return Retry<TException>(message, behavior, null, canceler);
    }

    /// <returns>Awaitable task handling the repetitions.</returns>
    /// <inheritdoc cref="Retry{TException,TResult}(string,Func{TResult},Action,CancellationToken?)"/>
    public Task Retry<TException>(
        string message, Action behavior, Action resetState, CancellationToken? canceler = null)
        where TException : Exception
    {
        return Retry<TException, bool>(message, () => { behavior?.Invoke(); return true; }, resetState, canceler);
    }

    /// <inheritdoc cref="Retry{TException,TResult}(string,Func{TResult},Action,CancellationToken?)"/>
    public Task<TResult> Retry<TResult>(string message, Func<TResult> behavior, CancellationToken? canceler = null)
    {
        return Retry<Exception, TResult>(message, behavior, null, canceler);
    }

    /// <inheritdoc cref="Retry{TException,TResult}(string,Func{TResult},Action,CancellationToken?)"/>
    public Task<TResult> Retry<TResult>(
        string message, Func<TResult> behavior, Action resetState, CancellationToken? canceler = null)
    {
        return Retry<Exception, TResult>(message, behavior, resetState, canceler);
    }

    /// <inheritdoc cref="Retry{TException,TResult}(string,Func{TResult},Action,CancellationToken?)"/>
    public Task<TResult> Retry<TException, TResult>(
        string message, Func<TResult> behavior, CancellationToken? canceler = null)
        where TException : Exception
    {
        return Retry<TException, TResult>(message, behavior, null, canceler);
    }

    /// <summary>Retries <paramref name="behavior"/> when encountering exceptions.</summary>
    /// <typeparam name="TException">Exception type to ignore and retry if encountered.</typeparam>
    /// <typeparam name="TResult">Result type returned from the behavior.</typeparam>
    /// <param name="message">Details to include upon a timeout exception.</param>
    /// <param name="behavior">Behavior to repeatably attempt.</param>
    /// <param name="resetState">Behavior to reset state before next attempt.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>Result of the successful <paramref name="behavior"/> attempt.</returns>
    /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
    /// <remarks>Beware infinite loops in <paramref name="behavior"/>.</remarks>
    public async Task<TResult> Retry<TException, TResult>(
        string message, Func<TResult> behavior, Action resetState, CancellationToken? canceler = null)
        where TException : Exception
    {
        ArgumentGuard.ThrowIfNull(behavior, nameof(behavior));

        Stopwatch watch = Stopwatch.StartNew();
        for (int i = 1; true; i++)
        {
            TException lastError;
            try
            {
                return behavior.Invoke();
            }
            catch (TException error)
            {
                lastError = error;
            }
            resetState?.Invoke();
            await DelayOrFault(message, watch.Elapsed, i, canceler, lastError).ConfigureAwait(false);
        }
    }

    /// <inheritdoc cref="Attempt{T}(string,Action,Action,CancellationToken?)"/>
    public Task Attempt(string message, Action behavior, CancellationToken? canceler = null)
    {
        return Attempt<Exception>(message, behavior, null, canceler);
    }

    /// <inheritdoc cref="Attempt{T}(string,Action,Action,CancellationToken?)"/>
    public Task Attempt(string message, Action behavior, Action resetState, CancellationToken? canceler = null)
    {
        return Attempt<Exception>(message, behavior, resetState, canceler);
    }

    /// <inheritdoc cref="Attempt{T}(string,Action,Action,CancellationToken?)"/>
    public Task Attempt<TException>(string message, Action behavior, CancellationToken? canceler = null)
        where TException : Exception
    {
        return Attempt<TException>(message, behavior, null, canceler);
    }

    /// <returns>Awaitable task handling the repetitions.</returns>
    /// <inheritdoc cref="Attempt{TException,TResult}(string,Func{TResult},Action,CancellationToken?)"/>
    public Task Attempt<TException>(
        string message, Action behavior, Action resetState, CancellationToken? canceler = null)
        where TException : Exception
    {
        return Attempt<TException, bool>(message, () => { behavior?.Invoke(); return true; }, resetState, canceler);
    }

    /// <inheritdoc cref="Attempt{TException,TResult}(string,Func{TResult},Action,CancellationToken?)"/>
    public Task<TResult> Attempt<TResult>(string message, Func<TResult> behavior, CancellationToken? canceler = null)
    {
        return Attempt<Exception, TResult>(message, behavior, null, canceler);
    }

    /// <inheritdoc cref="Attempt{TException,TResult}(string,Func{TResult},Action,CancellationToken?)"/>
    public Task<TResult> Attempt<TResult>(
        string message, Func<TResult> behavior, Action resetState, CancellationToken? canceler = null)
    {
        return Attempt<Exception, TResult>(message, behavior, resetState, canceler);
    }

    /// <inheritdoc cref="Attempt{TException,TResult}(string,Func{TResult},Action,CancellationToken?)"/>
    public Task<TResult> Attempt<TException, TResult>(
        string message, Func<TResult> behavior, CancellationToken? canceler = null)
        where TException : Exception
    {
        return Attempt<TException, TResult>(message, behavior, null, canceler);
    }

    /// <summary>Retries <paramref name="behavior"/> when encountering exceptions.</summary>
    /// <typeparam name="TException">Exception type to ignore and retry if encountered.</typeparam>
    /// <typeparam name="TResult">Result type returned from the behavior.</typeparam>
    /// <param name="message">Details to include upon a timeout exception.</param>
    /// <param name="behavior">Behavior to repeatably attempt.</param>
    /// <param name="resetState">Behavior to reset state before next attempt.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>Result of the successful <paramref name="behavior"/> attempt or default if limit reached.</returns>
    public async Task<TResult> Attempt<TException, TResult>(
        string message, Func<TResult> behavior, Action resetState, CancellationToken? canceler = null)
        where TException : Exception
    {
        ArgumentGuard.ThrowIfNull(behavior, nameof(behavior));

        Stopwatch watch = Stopwatch.StartNew();
        int i = 0;
        do
        {
            try
            {
                return behavior.Invoke();
            }
            catch (TException) { }

            resetState?.Invoke();
        } while (await DelayIfNotDone(message, watch.Elapsed, ++i, canceler).ConfigureAwait(false));

        return default;
    }

    /// <summary>Delays if terminal condition not reached.</summary>
    /// <param name="message">Details to include upon a timeout exception.</param>
    /// <param name="elapsed">Current amount of time that has elapsed.</param>
    /// <param name="tries">Current number of attempts.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>True if terminal condition not reached; false otherwise.</returns>
    private async Task<bool> DelayIfNotDone(string message, TimeSpan elapsed, int tries, CancellationToken? canceler)
    {
        if (tries < _tries && elapsed < _timeout)
        {
            await DelayOrCancel(message, canceler).ConfigureAwait(false);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>Faults if terminal condition reached; delays otherwise.</summary>
    /// <param name="message">Details to include upon a timeout exception.</param>
    /// <param name="elapsed">Current amount of time that has elapsed.</param>
    /// <param name="tries">Current number of attempts.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <param name="ex">Current exception if present.</param>
    /// <returns>Awaitable task handling the delay.</returns>
    /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
    private async Task DelayOrFault(
        string message, TimeSpan elapsed, int tries, CancellationToken? canceler, Exception ex = null)
    {
        string details = string.IsNullOrWhiteSpace(message) ? "." : $": {message}";
        if (tries >= _tries)
        {
            throw new TimeoutException($"Reached max attempts of '{_tries}'{details}", ex);
        }
        else if (elapsed >= _timeout)
        {
            throw new TimeoutException($"Reached timeout of '{_timeout}'{details}", ex);
        }
        else
        {
            await DelayOrCancel(message, canceler).ConfigureAwait(false);
        }
    }

    /// <summary>Faults if task has been canceled; delays otherwise.</summary>
    /// <param name="message">Details to include upon a timeout exception.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>Awaitable task handling the delay.</returns>
    private async Task DelayOrCancel(string message, CancellationToken? canceler)
    {
        CancellationToken token = canceler ?? CancellationToken.None;
        try
        {
            if (_delay > TimeSpan.Zero)
            {
                await Task.Delay(_delay, token).ConfigureAwait(false);
            }
            else
            {
                token.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException e)
        {
            string details = string.IsNullOrWhiteSpace(message) ? "." : $": {message}";
            throw new OperationCanceledException($"Operation canceled via token{details}", e, canceler.Value);
        }
    }

    /// <summary>Compares to <paramref name="obj"/> by value.</summary>
    /// <param name="obj">Instance to compare with.</param>
    /// <returns>True if equal to <paramref name="obj"/> by value; false otherwise.</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as Limiter);
    }

    /// <summary>Compares to <paramref name="other"/> by value.</summary>
    /// <param name="other">Instance to compare with.</param>
    /// <returns>True if equal to <paramref name="other"/> by value; false otherwise.</returns>
    public bool Equals(Limiter other)
    {
        return other != null
            && other._delay == _delay
            && other._tries == _tries
            && other._timeout == _timeout;
    }

    /// <summary>Computes a hash code based upon value.</summary>
    /// <returns>The computed hash code.</returns>
    public override int GetHashCode()
    {
        return ValueComparer.Use.GetHashCode(_tries, _timeout, _delay);
    }

    /// <summary>Converts this object to a string.</summary>
    /// <returns>String representation of the object.</returns>
    public override string ToString()
    {
        return $"{_tries}-{_timeout}-{_delay}";
    }
}
