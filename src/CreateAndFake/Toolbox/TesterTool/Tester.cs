using System;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates common tests.</summary>
    public class Tester
    {
        /// <summary>Default for how long to wait for methods to complete.</summary>
        private static readonly TimeSpan _DefaultTimeout = new TimeSpan(0, 0, 3);

        /// <summary>Retries tests if timeout is reached.</summary>
        private static readonly Limiter _Limiter = Limiter.Few;

        /// <summary>Core value random handler.</summary>
        protected IRandom Gen { get; }

        /// <summary>Creates objects and populates them with random values.</summary>
        protected IRandomizer Randomizer { get; }

        /// <summary>Deep clones objects.</summary>
        protected IDuplicator Duplicator { get; }

        /// <summary>Handles common test scenarios.</summary>
        protected Asserter Asserter { get; }

        /// <summary>How long to wait for tests to complete.</summary>
        private readonly TimeSpan _timeout;

        /// <summary>Sets up the tester capabilities.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="duplicator">Deep clones objects.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <param name="timeout">How long to wait for tests to complete.</param>
        /// <exception cref="ArgumentNullException">If given nulls.</exception>
        public Tester(IRandom gen, IRandomizer randomizer,
            IDuplicator duplicator, Asserter asserter, TimeSpan? timeout = null)
        {
            Gen = gen ?? throw new ArgumentNullException(nameof(gen));
            Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            Duplicator = duplicator ?? throw new ArgumentNullException(nameof(duplicator));
            Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
            _timeout = timeout ?? _DefaultTimeout;
        }

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
            if (type == null) throw new ArgumentNullException(nameof(type));

            NullGuarder checker = new NullGuarder(
                new GenericFixer(Gen, Randomizer), Randomizer, Asserter, _timeout);

            _Limiter.Retry<TimeoutException>(
                () => checker.PreventsNullRefExceptionOnConstructors(type, true, injectionValues)).Wait();

            if (!(type.IsAbstract && type.IsSealed))
            {
                _Limiter.Retry<TimeoutException>(() =>
                {
                    object instance = Randomizer.Create(type);
                    try
                    {
                        checker.PreventsNullRefExceptionOnMethods(instance, injectionValues);
                    }
                    finally
                    {
                        (instance as IDisposable)?.Dispose();
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
            NullGuarder checker = new NullGuarder(
                new GenericFixer(Gen, Randomizer), Randomizer, Asserter, _timeout);

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
            if (type == null) throw new ArgumentNullException(nameof(type));

            MutationGuarder checker = new MutationGuarder(
                new GenericFixer(Gen, Randomizer), Randomizer, Duplicator, Asserter, _timeout);

            _Limiter.Retry<TimeoutException>(
                () => checker.PreventsMutationOnConstructors(type, true, injectionValues)).Wait();

            if (!(type.IsAbstract && type.IsSealed))
            {
                _Limiter.Retry<TimeoutException>(() =>
                {
                    object instance = Randomizer.Create(type);
                    try
                    {
                        checker.PreventsMutationOnMethods(instance, injectionValues);
                    }
                    finally
                    {
                        (instance as IDisposable)?.Dispose();
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
            MutationGuarder checker = new MutationGuarder(
                new GenericFixer(Gen, Randomizer), Randomizer, Duplicator, Asserter, _timeout);

            _Limiter.Retry<TimeoutException>(
                () => checker.PreventsMutationOnConstructors(typeof(T), false, injectionValues)).Wait();
            _Limiter.Retry<TimeoutException>(
                () => checker.PreventsMutationOnMethods(instance, injectionValues)).Wait();
            _Limiter.Retry<TimeoutException>(
                () => checker.PreventsMutationOnStatics(typeof(T), false, injectionValues)).Wait();
        }
    }
}
