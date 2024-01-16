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
    /// <inheritdoc cref="Repeat{T}(Func{T},CancellationToken?)"/>
    public async Task Repeat(Action behavior, CancellationToken? canceler = null)
    {
        _ = await Repeat(() => { behavior?.Invoke(); return true; }, canceler).ConfigureAwait(false);
    }

    /// <summary>Repeats <paramref name="behavior"/>.</summary>
    /// <typeparam name="T">Result type returned from the action.</typeparam>
    /// <param name="behavior">Behavior to repeatably do.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>Results from all calls.</returns>
    public async Task<IReadOnlyCollection<T>> Repeat<T>(Func<T> behavior, CancellationToken? canceler = null)
    {
        ArgumentGuard.ThrowIfNull(behavior, nameof(behavior));

        List<T> results = [];

        Stopwatch watch = Stopwatch.StartNew();
        int attempt = 0;
        do
        {
            results.Add(behavior.Invoke());
        } while (await DelayIfNotDone(watch.Elapsed, ++attempt, canceler).ConfigureAwait(false));

        return results.AsReadOnly();
    }

    /// <summary>Retries <paramref name="behavior"/> until it returns true.</summary>
    /// <inheritdoc cref="StallUntil(Action,Func{bool},CancellationToken?)"/>
    public async Task StallUntil(Func<bool> behavior, CancellationToken? canceler = null)
    {
        await StallUntil(null, behavior, canceler).ConfigureAwait(false);
    }

    /// <summary>Retries <paramref name="behavior"/> until <paramref name="checkState"/>.</summary>
    /// <returns>Awaitable task handling the repetitions.</returns>
    /// <inheritdoc cref="StallUntil{T}(Func{T},Func{bool},CancellationToken?)"/>
    public async Task StallUntil(Action behavior, Func<bool> checkState, CancellationToken? canceler = null)
    {
        _ = await StallUntil(() => { behavior?.Invoke(); return true; }, checkState, canceler).ConfigureAwait(false);
    }

    /// <summary>Retries <paramref name="behavior"/> until <paramref name="checkState"/>.</summary>
    /// <typeparam name="T">Result type returned from the action.</typeparam>
    /// <param name="behavior">Behavior to repeatably attempt.</param>
    /// <param name="checkState">Polls if the behavior was successful.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>Results from all calls.</returns>
    /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
    public async Task<IReadOnlyCollection<T>> StallUntil<T>(
        Func<T> behavior, Func<bool> checkState, CancellationToken? canceler = null)
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
                await DelayOrFault(watch.Elapsed, i, canceler).ConfigureAwait(false);
            }
        }

        return results.AsReadOnly();
    }

    /// <inheritdoc cref="Retry{T}(Action,Action,CancellationToken?)"/>
    public async Task Retry(Action behavior, CancellationToken? canceler = null)
    {
        await Retry<Exception>(behavior, null, canceler).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Retry{T}(Action,Action,CancellationToken?)"/>
    public async Task Retry(Action behavior, Action resetState, CancellationToken? canceler = null)
    {
        await Retry<Exception>(behavior, resetState, canceler).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Retry{T}(Action,Action,CancellationToken?)"/>
    public async Task Retry<TException>(Action behavior, CancellationToken? canceler = null)
        where TException : Exception
    {
        await Retry<TException>(behavior, null, canceler).ConfigureAwait(false);
    }

    /// <returns>Awaitable task handling the repetitions.</returns>
    /// <inheritdoc cref="Retry{TException,TResult}(Func{TResult},Action,CancellationToken?)"/>
    public async Task Retry<TException>(Action behavior, Action resetState, CancellationToken? canceler = null)
        where TException : Exception
    {
        _ = await Retry<TException, bool>(() => { behavior?.Invoke(); return true; }, resetState, canceler).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Retry{TException,TResult}(Func{TResult},Action,CancellationToken?)"/>
    public async Task<TResult> Retry<TResult>(Func<TResult> behavior, CancellationToken? canceler = null)
    {
        return await Retry<Exception, TResult>(behavior, null, canceler).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Retry{TException,TResult}(Func{TResult},Action,CancellationToken?)"/>
    public async Task<TResult> Retry<TResult>(
        Func<TResult> behavior, Action resetState, CancellationToken? canceler = null)
    {
        return await Retry<Exception, TResult>(behavior, resetState, canceler).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Retry{TException,TResult}(Func{TResult},Action,CancellationToken?)"/>
    public async Task<TResult> Retry<TException, TResult>(Func<TResult> behavior, CancellationToken? canceler = null)
        where TException : Exception
    {
        return await Retry<TException, TResult>(behavior, null, canceler).ConfigureAwait(false);
    }

    /// <summary>Retries <paramref name="behavior"/> when encountering exceptions.</summary>
    /// <typeparam name="TException">Exception type to ignore and retry if encountered.</typeparam>
    /// <typeparam name="TResult">Result type returned from the behavior.</typeparam>
    /// <param name="behavior">Behavior to repeatably attempt.</param>
    /// <param name="resetState">Behavior to reset state before next attempt.</param>
    /// <param name="canceler">Token to potentially cancel the attempts.</param>
    /// <returns>Result of the successful <paramref name="behavior"/> attempt.</returns>
    /// <exception cref="TimeoutException">If an attempt limit is reached.</exception>
    public async Task<TResult> Retry<TException, TResult>(
        Func<TResult> behavior, Action resetState, CancellationToken? canceler = null)
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
            await DelayOrFault(watch.Elapsed, i, canceler, lastError).ConfigureAwait(false);
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
    /// <returns>Awaitable task handling the delay.</returns>
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
    /// <returns>Awaitable task handling the delay.</returns>
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
