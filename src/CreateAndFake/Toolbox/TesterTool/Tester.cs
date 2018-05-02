using System;
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
        private static readonly TimeSpan s_DefaultTimeout = new TimeSpan(0, 0, 5);

        /// <summary>Core value random handler.</summary>
        protected IRandom Gen { get; }

        /// <summary>Creates objects and populates them with random values.</summary>
        protected IRandomizer Randomizer { get; }

        /// <summary>Deep clones objects.</summary>
        protected IDuplicator Duplicator { get; }

        /// <summary>Handles common test scenarios.</summary>
        protected Asserter Asserter { get; }

        /// <summary>How long to wait for tests to complete.</summary>
        private readonly TimeSpan m_Timeout;

        /// <summary>Sets up the asserter capabilities.</summary>
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
            m_Timeout = timeout ?? s_DefaultTimeout;
        }

        /// <summary>Verifies nulls are guarded on the type.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        /// <remarks>
        ///     Tests each nullable parameter possible with null.
        ///     Constructor and factory parameters are additionally tested by running all methods.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </remarks>
        public virtual void PreventsNullRefException<T>()
        {
            PreventsNullRefException(typeof(T));
        }

        /// <summary>Verifies nulls are guarded on the type.</summary>
        /// <param name="type">Type to verify.</param>
        /// <remarks>
        ///     Tests each nullable parameter possible with null.
        ///     Constructor and factory parameters are additionally tested by running all methods.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </remarks>
        public virtual void PreventsNullRefException(Type type)
        {
            NullGuarder checker = new NullGuarder(
                new GenericFixer(Gen, Randomizer), Randomizer, Asserter, m_Timeout);

            checker.PreventsNullRefExceptionOnConstructors(type, true);

            if (!(type.IsAbstract && type.IsSealed))
            {
                object instance = Randomizer.Create(type);
                checker.PreventsNullRefExceptionOnMethods(instance);
                (instance as IDisposable)?.Dispose();
            }

            checker.PreventsNullRefExceptionOnStatics(type, true);
        }

        /// <summary>Verifies nulls are guarded on the type.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        /// <param name="instance">Instance to test the methods on.</param>
        /// <remarks>
        ///     Tests each parameter possible with null.
        ///     Constructor and factory parameters are not tested by running all methods.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </remarks>
        public virtual void PreventsNullRefException<T>(T instance)
        {
            NullGuarder checker = new NullGuarder(
                new GenericFixer(Gen, Randomizer), Randomizer, Asserter, m_Timeout);

            checker.PreventsNullRefExceptionOnConstructors(typeof(T), false);
            checker.PreventsNullRefExceptionOnMethods(instance);
            checker.PreventsNullRefExceptionOnStatics(typeof(T), false);
        }

        /// <summary>Verifies mutations are prevented on the type.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        /// <remarks>
        ///     Constructor and factory parameters are additionally tested by running all methods.
        ///     Ignores any exception and moves on.
        /// </remarks>
        public virtual void PreventsParameterMutation<T>()
        {
            PreventsParameterMutation(typeof(T));
        }

        /// <summary>Verifies mutations are prevented on the type.</summary>
        /// <param name="type">Type to verify.</param>
        /// <remarks>
        ///     Constructor and factory parameters are additionally tested by running all methods.
        ///     Ignores any exception and moves on.
        /// </remarks>
        public virtual void PreventsParameterMutation(Type type)
        {
            MutationGuarder checker = new MutationGuarder(
                new GenericFixer(Gen, Randomizer), Randomizer, Duplicator, Asserter, m_Timeout);

            checker.PreventsMutationOnConstructors(type, true);

            if (!(type.IsAbstract && type.IsSealed))
            {
                object instance = Randomizer.Create(type);
                checker.PreventsMutationOnMethods(instance);
                (instance as IDisposable)?.Dispose();
            }

            checker.PreventsMutationOnStatics(type, true);
        }

        /// <summary>Verifies mutations are prevented on the type.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        /// <param name="instance">Instance to test the methods on.</param>
        /// <remarks>
        ///     Constructor and factory parameters are not tested by running all methods.
        ///     Ignores any exception and moves on.
        /// </remarks>
        public virtual void PreventsParameterMutation<T>(T instance)
        {
            MutationGuarder checker = new MutationGuarder(
                new GenericFixer(Gen, Randomizer), Randomizer, Duplicator, Asserter, m_Timeout);

            checker.PreventsMutationOnConstructors(typeof(T), false);
            checker.PreventsMutationOnMethods(instance);
            checker.PreventsMutationOnStatics(typeof(T), false);
        }
    }
}
