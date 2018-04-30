using System;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates common tests.</summary>
    public class Tester
    {
        /// <summary>Core value random handler.</summary>
        protected IRandom Gen { get; }

        /// <summary>Creates objects and populates them with random values.</summary>
        protected IRandomizer Randomizer { get; }

        /// <summary>Handles common test scenarios.</summary>
        protected Asserter Asserter { get; }

        /// <summary>Sets up the asserter capabilities.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <exception cref="ArgumentNullException">If given nulls.</exception>
        public Tester(IRandom gen, IRandomizer randomizer, Asserter asserter)
        {
            Gen = gen ?? throw new ArgumentNullException(nameof(gen));
            Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
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
            NullGuarder checker = new NullGuarder(Gen, Randomizer, Asserter);

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
            NullGuarder checker = new NullGuarder(Gen, Randomizer, Asserter);

            checker.PreventsNullRefExceptionOnConstructors(typeof(T), false);
            checker.PreventsNullRefExceptionOnMethods(instance);
            checker.PreventsNullRefExceptionOnStatics(typeof(T), false);
        }
    }
}
