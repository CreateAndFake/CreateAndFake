using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Provides a callback into the randomizer to create child values.</summary>
    public sealed class RandomizerChainer
    {
        /// <summary>Callback to the randomizer to create child values.</summary>
        private readonly Func<Type, RandomizerChainer, object> _randomizer;

        /// <summary>Types not to create as to prevent infinite recursion.</summary>
        private readonly IEnumerable<Type> _history;

        /// <summary>Provides stubs.</summary>
        private readonly IFaker _faker;

        /// <summary>Value generator to use for base randomization.</summary>
        public IRandom Gen { get; }

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="faker">Provides stubs.</param>
        /// <param name="gen">Value generator to use for base randomization.</param>
        /// <param name="randomizer">Callback to the randomizer to create child values.</param>
        public RandomizerChainer(IFaker faker, IRandom gen, Func<Type, RandomizerChainer, object> randomizer)
        {
            _faker = faker ?? throw new ArgumentNullException(nameof(faker));
            Gen = gen ?? throw new ArgumentNullException(nameof(gen));
            _randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));

            _history = Array.Empty<Type>();
        }

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="prevChainer">Previous chainer to build upon.</param>
        /// <param name="createdContainer">Container of the instance just created.</param>
        private RandomizerChainer(RandomizerChainer prevChainer, Type createdContainer)
        {
            Gen = prevChainer.Gen;
            _faker = prevChainer._faker;
            _randomizer = prevChainer._randomizer;

            if (createdContainer != null)
            {
                _history = new HashSet<Type>(prevChainer._history.Append(createdContainer));
            }
            else
            {
                _history = prevChainer._history;
            }
        }

        /// <summary>Checks if a type has already been created by the randomizer.</summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if already created; false otherwise.</returns>
        public bool AlreadyCreated<T>()
        {
            return AlreadyCreated(typeof(T));
        }

        /// <summary>Checks if a type has already been created by the randomizer.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if already created; false otherwise.</returns>
        public bool AlreadyCreated(Type type)
        {
            return _history.Contains(type);
        }

        /// <summary>Calls the randomizer to create a random instance.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="createdContainer">Container of the instance to create.</param>
        /// <returns>The created instance.</returns>
        public T Create<T>(Type createdContainer = null)
        {
            return (T)Create(typeof(T), createdContainer);
        }

        /// <summary>Calls the randomizer to create a random instance.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="createdContainer">Container of the instance to create.</param>
        /// <returns>The created instance.</returns>
        public object Create(Type type, Type createdContainer = null)
        {
            if (AlreadyCreated(type))
            {
                throw new InfiniteLoopException(type, _history);
            }

            RuntimeHelpers.EnsureSufficientExecutionStack();
            return _randomizer.Invoke(type, new RandomizerChainer(this, createdContainer));
        }

        /// <summary>Calls the faker to create a stub instance.</summary>
        /// <typeparam name="T">Type to stub.</typeparam>
        /// <returns>The stubbed instance.</returns>
        public Fake<T> Stub<T>()
        {
            return _faker.Stub<T>();
        }

        /// <summary>Calls the faker to create a stub instance.</summary>
        /// <param name="parent">Type to stub.</param>
        /// <returns>The stubbed instance.</returns>
        public Fake Stub(Type parent)
        {
            return _faker.Stub(parent);
        }

        /// <summary>Determines if the type can be faked.</summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if possible; false otherwise.</returns>
        public bool FakerSupports<T>()
        {
            return _faker.Supports<T>();
        }

        /// <summary>Determines if the type can be faked.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if possible; false otherwise.</returns>
        public bool FakerSupports(Type type)
        {
            return _faker.Supports(type);
        }
    }
}
