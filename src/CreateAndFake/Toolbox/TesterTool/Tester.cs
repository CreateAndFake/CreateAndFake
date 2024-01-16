using System;
using CreateAndFake.Design;
using CreateAndFake.Design.Content;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool;

/// <summary>Automates common tests.</summary>
/// <param name="gen">Core value random handler.</param>
/// <param name="randomizer">Creates objects and populates them with random values.</param>
/// <param name="duplicator">Deep clones objects.</param>
/// <param name="asserter">Handles common test scenarios.</param>
/// <param name="timeout">How long to wait for tests to complete.</param>
/// <exception cref="ArgumentNullException">If given nulls.</exception>
public class Tester(IRandom gen, IRandomizer randomizer,
    IDuplicator duplicator, Asserter asserter, TimeSpan? timeout = null)
{
    /// <summary>Retries tests if timeout is reached.</summary>
    private static readonly Limiter _Limiter = Limiter.Few;

    /// <summary>Core value random handler.</summary>
    protected IRandom Gen { get; } = gen ?? throw new ArgumentNullException(nameof(gen));

    /// <summary>Creates objects and populates them with random values.</summary>
    protected IRandomizer Randomizer { get; } = randomizer ?? throw new ArgumentNullException(nameof(randomizer));

    /// <summary>Deep clones objects.</summary>
    protected IDuplicator Duplicator { get; } = duplicator ?? throw new ArgumentNullException(nameof(duplicator));

    /// <summary>Handles common test scenarios.</summary>
    protected Asserter Asserter { get; } = asserter ?? throw new ArgumentNullException(nameof(asserter));

    /// <summary>How long to wait for tests to complete.</summary>
    private readonly TimeSpan _timeout = timeout ?? new(0, 0, 3);

    /// <summary>
    ///     Verifies nulls are guarded on the type.
    ///     Tests each nullable parameter possible with null.
    ///     Constructor and factory parameters are additionally tested by running all methods.
    ///     Ignores any exception besides NullReferenceException and moves on.
    /// </summary>
    /// <typeparam name="T">Type to verify.</typeparam>
    /// <param name="injectionValues">Values to inject into the method.</param>
    public virtual void PreventsNullRefException<T>(params object[] injectionValues)
    {
        PreventsNullRefException(typeof(T), injectionValues);
    }

    /// <summary>
    ///     Verifies nulls are guarded on the type.
    ///     Tests each nullable parameter possible with null.
    ///     Constructor and factory parameters are additionally tested by running all methods.
    ///     Ignores any exception besides NullReferenceException and moves on.
    /// </summary>
    /// <param name="type">Type to verify.</param>
    /// <param name="injectionValues">Values to inject into the method.</param>
    public virtual void PreventsNullRefException(Type type, params object[] injectionValues)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        NullGuarder checker = new(new GenericFixer(Gen, Randomizer), Randomizer, Asserter, _timeout);

        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsNullRefExceptionOnConstructors(type, true, injectionValues)).Wait();

        if (!(type.IsAbstract && type.IsSealed))
        {
            _Limiter.Retry<TimeoutException>(() =>
            {
                object instance = (injectionValues?.Length > 0)
                    ? Randomizer.Inject(type, injectionValues)
                    : Randomizer.Create(type);
                try
                {
                    checker.PreventsNullRefExceptionOnMethods(instance, injectionValues);
                }
                finally
                {
                    Disposer.Cleanup(instance);
                }
            }).Wait();
        }

        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsNullRefExceptionOnStatics(type, true, injectionValues)).Wait();
    }

    /// <summary>
    ///     Verifies nulls are guarded on the type.
    ///     Tests each parameter possible with null.
    ///     Constructor and factory parameters are not tested by running all methods.
    ///     Ignores any exception besides NullReferenceException and moves on.
    /// </summary>
    /// <typeparam name="T">Type to verify.</typeparam>
    /// <param name="instance">Instance to test the methods on.</param>
    /// <param name="injectionValues">Values to inject into the method.</param>
    public virtual void PreventsNullRefException<T>(T instance, params object[] injectionValues)
    {
        NullGuarder checker = new(new GenericFixer(Gen, Randomizer), Randomizer, Asserter, _timeout);

        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsNullRefExceptionOnConstructors(typeof(T), false, injectionValues)).Wait();
        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsNullRefExceptionOnMethods(instance, injectionValues)).Wait();
        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsNullRefExceptionOnStatics(typeof(T), false, injectionValues)).Wait();
    }

    /// <summary>
    ///     Verifies mutations are prevented on the type.
    ///     Constructor and factory parameters are additionally tested by running all methods.
    ///     Ignores any exception and moves on.
    /// </summary>
    /// <typeparam name="T">Type to verify.</typeparam>
    /// <param name="injectionValues">Values to inject into the method.</param>
    public virtual void PreventsParameterMutation<T>(params object[] injectionValues)
    {
        PreventsParameterMutation(typeof(T), injectionValues);
    }

    /// <summary>
    ///     Verifies mutations are prevented on the type.
    ///     Constructor and factory parameters are additionally tested by running all methods.
    ///     Ignores any exception and moves on.
    /// </summary>
    /// <param name="type">Type to verify.</param>
    /// <param name="injectionValues">Values to inject into the method.</param>
    public virtual void PreventsParameterMutation(Type type, params object[] injectionValues)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        MutationGuarder checker = new(new GenericFixer(Gen, Randomizer),
            Randomizer, Duplicator, Asserter, _timeout);

        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsMutationOnConstructors(type, true, injectionValues)).Wait();

        if (!(type.IsAbstract && type.IsSealed))
        {
            _Limiter.Retry<TimeoutException>(() =>
            {
                object instance = (injectionValues?.Length > 0)
                    ? Randomizer.Inject(type, injectionValues)
                    : Randomizer.Create(type);
                try
                {
                    checker.PreventsMutationOnMethods(instance, injectionValues);
                }
                finally
                {
                    Disposer.Cleanup(instance);
                }
            }).Wait();
        }

        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsMutationOnStatics(type, true, injectionValues)).Wait();
    }

    /// <summary>
    ///     Verifies mutations are prevented on the type.
    ///     Constructor and factory parameters are not tested by running all methods.
    ///     Ignores any exception and moves on.
    /// </summary>
    /// <typeparam name="T">Type to verify.</typeparam>
    /// <param name="instance">Instance to test the methods on.</param>
    /// <param name="injectionValues">Values to inject into the method.</param>
    public virtual void PreventsParameterMutation<T>(T instance, params object[] injectionValues)
    {
        MutationGuarder checker = new(new GenericFixer(Gen, Randomizer),
            Randomizer, Duplicator, Asserter, _timeout);

        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsMutationOnConstructors(typeof(T), false, injectionValues)).Wait();
        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsMutationOnMethods(instance, injectionValues)).Wait();
        _Limiter.Retry<TimeoutException>(
            () => checker.PreventsMutationOnStatics(typeof(T), false, injectionValues)).Wait();
    }

    /// <summary>Verifies no exceptions are thrown on any method when using injection and random data.</summary>
    /// <typeparam name="T">Type to verify.</typeparam>
    /// <param name="injectionValues">Values to inject into called methods.</param>
    public virtual void PassthroughWithNoExceptions<T>(params object[] injectionValues)
    {
        PassthroughWithNoExceptions(Randomizer.Create<Injected<T>>().Dummy, injectionValues);
    }

    /// <summary>Verifies no exceptions are thrown on any method.</summary>
    /// <param name="instance">Instance to test the methods on.</param>
    /// <param name="injectionValues">Values to inject into called methods.</param>
    public virtual void PassthroughWithNoExceptions(object instance, params object[] injectionValues)
    {
        new ExceptionGuarder(new GenericFixer(Gen, Randomizer), Randomizer, Asserter, _timeout)
            .CallAllMethods(instance, injectionValues);
    }
}
