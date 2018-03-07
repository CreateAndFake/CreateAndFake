using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Provides a callback into the randomizer to create child values.</summary>
    public sealed class RandomizerChainer
    {
        /// <summary>Callback to the randomizer to create child values.</summary>
        private readonly Func<Type, IEnumerable<Type>, object> m_Randomizer;

        /// <summary>Types not to create as to prevent infinite recursion.</summary>
        private readonly IEnumerable<Type> m_History;

        /// <summary>Provides stubs.</summary>
        private readonly IFaker m_Faker;

        /// <summary>Value generator to use for base randomization.</summary>
        public IRandom Gen { get; }

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="faker">Provides stubs.</param>
        /// <param name="gen">Value generator to use for base randomization.</param>
        /// <param name="history">Types not to create as to prevent infinite recursion.</param>
        /// <param name="randomizer">Callback to the randomizer to create child values.</param>
        public RandomizerChainer(IFaker faker, IRandom gen, IEnumerable<Type> history,
            Func<Type, IEnumerable<Type>, object> randomizer)
        {
            m_Faker = faker ?? throw new ArgumentNullException(nameof(faker));
            Gen = gen ?? throw new ArgumentNullException(nameof(randomizer));
            m_Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));

            m_History = history ?? Enumerable.Empty<Type>();
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
            return m_History.Contains(type);
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
            return m_Randomizer.Invoke(type, m_History.Append(createdContainer));
        }

        /// <summary>Calls the faker to create a stub instance.</summary>
        /// <typeparam name="T">Type to stub.</typeparam>
        /// <returns>The stubbed instance.</returns>
        public Fake<T> Stub<T>()
        {
            return m_Faker.Stub<T>();
        }

        /// <summary>Calls the faker to create a stub instance.</summary>
        /// <param name="parent">Type to stub.</param>
        /// <returns>The stubbed instance.</returns>
        public Fake Stub(Type parent)
        {
            return m_Faker.Stub(parent);
        }
    }
}
