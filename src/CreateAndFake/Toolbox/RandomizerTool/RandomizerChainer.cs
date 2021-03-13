using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Provides a callback into <see cref="IRandomizer"/> to create child values.</summary>
    public sealed class RandomizerChainer
    {
        /// <summary>Callback to the randomizer to create child values.</summary>
        private readonly Func<Type, RandomizerChainer, object> _randomizer;

        /// <summary>Types not to create as to prevent infinite recursion.</summary>
        private readonly IDictionary<Type, object> _history;

        /// <summary>Provides stubs.</summary>
        private readonly IFaker _faker;

        /// <summary>Value generator to use for base randomization.</summary>
        public IRandom Gen { get; }

        /// <summary>Created object using this instance.</summary>
        public object Parent { get; }

        /// <summary>Initializes a new instance of the <see cref="RandomizerChainer"/> class.</summary>
        /// <param name="faker">Provides stubs.</param>
        /// <param name="gen">Value generator to use for base randomization.</param>
        /// <param name="randomizer">Callback to the randomizer to create child values.</param>
        public RandomizerChainer(IFaker faker, IRandom gen, Func<Type, RandomizerChainer, object> randomizer)
        {
            _faker = faker ?? throw new ArgumentNullException(nameof(faker));
            Gen = gen ?? throw new ArgumentNullException(nameof(gen));
            _randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));

            _history = new Dictionary<Type, object>();
            Parent = null;
        }

        /// <summary>Initializes a new instance of the <see cref="RandomizerChainer"/> class.</summary>
        /// <param name="prevChainer">Previous chainer to build upon.</param>
        /// <param name="parent">Container of the instance to create.</param>
        private RandomizerChainer(RandomizerChainer prevChainer, object parent)
        {
            Parent = parent;
            Gen = prevChainer.Gen;
            _faker = prevChainer._faker;
            _randomizer = prevChainer._randomizer;

            if (parent != null)
            {
                _history = prevChainer._history
                    .Append(new KeyValuePair<Type, object>(parent.GetType(), parent))
                    .ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                _history = prevChainer._history;
            }
        }

        /// <summary>Checks if <typeparamref name="T"/> has already been created by the randomizer.</summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if <typeparamref name="T"/> already created; false otherwise.</returns>
        public bool AlreadyCreated<T>()
        {
            return AlreadyCreated(typeof(T));
        }

        /// <summary>Checks if <paramref name="type"/> has already been created by the randomizer.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if <paramref name="type"/> already created; false otherwise.</returns>
        public bool AlreadyCreated(Type type)
        {
            return _history.ContainsKey(type);
        }

        /// <summary>Calls the randomizer to create a random <typeparamref name="T"/> instance.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <returns>The created <typeparamref name="T"/> instance.</returns>
        public T Create<T>()
        {
            return (T)Create(typeof(T), null);
        }

        /// <summary>Calls the randomizer to create a random instance.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="parent">Container of the instance to create.</param>
        /// <returns>The created instance.</returns>
        public object Create(Type type, object parent = null)
        {
            if (parent != null)
            {
                if (AlreadyCreated(type))
                {
                    return _history[type];
                }
                else if (parent.GetType() == type)
                {
                    return parent;
                }
            }
            else if (AlreadyCreated(type))
            {
                throw new InfiniteLoopException(type, _history.Keys);
            }

            RuntimeHelpers.EnsureSufficientExecutionStack();
            return _randomizer.Invoke(type, new RandomizerChainer(this, (parent != Parent) ? parent : null));
        }

        /// <summary>Calls the faker to create a stub <typeparamref name="T"/> instance.</summary>
        /// <typeparam name="T">Type to stub.</typeparam>
        /// <returns>The stubbed <typeparamref name="T"/> instance.</returns>
        public Fake<T> Stub<T>()
        {
            return _faker.Stub<T>();
        }

        /// <summary>Calls the faker to create a stub instance.</summary>
        /// <param name="type">Type to stub.</param>
        /// <returns>The stubbed instance.</returns>
        public Fake Stub(Type type)
        {
            return _faker.Stub(type);
        }

        /// <summary>Determines if <typeparamref name="T"/> can be faked.</summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if possible; false otherwise.</returns>
        public bool FakerSupports<T>()
        {
            return _faker.Supports<T>();
        }

        /// <summary>Determines if <paramref name="type"/> can be faked.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if possible; false otherwise.</returns>
        public bool FakerSupports(Type type)
        {
            return _faker.Supports(type);
        }
    }
}
