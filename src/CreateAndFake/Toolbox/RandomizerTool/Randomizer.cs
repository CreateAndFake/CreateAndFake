using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Creates objects and populates them with random values.</summary>
    public sealed class Randomizer : IRandomizer, IDuplicatable
    {
        /// <summary>Default set of hints to use for randomization.</summary>
        private static readonly CreateHint[] s_DefaultHints = new CreateHint[]
        {
            new ValueCreateHint(),
            new EnumCreateHint(),
            new GenericCreateHint(),
            new CollectionCreateHint(),
            new LegacyCollectionCreateHint(),
            new StringCreateHint(),
            new DelegateCreateHint(),
            new CommonSystemCreateHint(),
            new FakedCreateHint(),
            new ExceptionCreateHint(),
            new ObjectCreateHint()
        };

        /// <summary>Provides stubs.</summary>
        private readonly IFaker m_Faker;

        /// <summary>Generators used to randomize specific types.</summary>
        internal IEnumerable<CreateHint> Hints { get; }

        /// <summary>Value generator used for base randomization.</summary>
        public IRandom Gen { get; }

        /// <summary>Sets up the randomizer capabilities.</summary>
        /// <param name="faker">Provides stubs.</param>
        /// <param name="gen">Value generator to use for base randomization.</param>
        /// <param name="includeDefaultHints">If the default set of hints should be added.</param>
        /// <param name="hints">Generators used to randomize specific types.</param>
        public Randomizer(IFaker faker, IRandom gen, bool includeDefaultHints = true, params CreateHint[] hints)
        {
            m_Faker = faker ?? throw new ArgumentNullException(nameof(faker));
            Gen = gen ?? throw new ArgumentNullException(nameof(gen));

            var inputHints = hints ?? Enumerable.Empty<CreateHint>();
            if (includeDefaultHints)
            {
                Hints = inputHints.Concat(s_DefaultHints).ToArray();
            }
            else
            {
                Hints = inputHints.ToArray();
            }
        }

        /// <summary>Creates a randomized instance.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        /// <summary>Creates a randomized instance.</summary>
        /// <param name="type">Type to create.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        public object Create(Type type)
        {
            return Create(type, new RandomizerChainer(m_Faker, Gen, Create));
        }

        /// <summary>Creates a randomized instance.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="chainer">Handles callback behavior for child values.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        private object Create(Type type, RandomizerChainer chainer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            (bool, object) result = Hints
                .Select(h => h.TryCreate(type, chainer))
                .FirstOrDefault(r => r.Item1);

            if (!result.Equals(default))
            {
                return result.Item2;
            }
            else
            {
                throw new NotSupportedException(
                    "Type '" + type.FullName + "' not supported by the randomizer. " +
                    "Create a hint to generate the type and pass it to the randomizer.");
            }
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <param name="duplicator">Duplicator to clone child values.</param>
        /// <returns>Clone that is equal in value to the instance.</returns>
        public IDuplicatable DeepClone(IDuplicator duplicator)
        {
            return new Randomizer(duplicator.Copy(m_Faker),
                duplicator.Copy(Gen), false, duplicator.Copy(Hints).ToArray());
        }
    }
}
