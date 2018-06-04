using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            new FakeCreateHint(),
            new FakedCreateHint(),
            new ExceptionCreateHint(),
            new ObjectCreateHint()
        };

        /// <summary>Provides stubs.</summary>
        private readonly IFaker m_Faker;

        /// <summary>Generators used to randomize specific types.</summary>
        private readonly IEnumerable<CreateHint> m_Hints;

        /// <summary>Value generator used for base randomization.</summary>
        private readonly IRandom m_Gen;

        /// <summary>Sets up the randomizer capabilities.</summary>
        /// <param name="faker">Provides stubs.</param>
        /// <param name="gen">Value generator to use for base randomization.</param>
        /// <param name="includeDefaultHints">If the default set of hints should be added.</param>
        /// <param name="hints">Generators used to randomize specific types.</param>
        public Randomizer(IFaker faker, IRandom gen, bool includeDefaultHints = true, params CreateHint[] hints)
        {
            m_Faker = faker ?? throw new ArgumentNullException(nameof(faker));
            m_Gen = gen ?? throw new ArgumentNullException(nameof(gen));

            var inputHints = hints ?? Enumerable.Empty<CreateHint>();
            if (includeDefaultHints)
            {
                m_Hints = inputHints.Concat(s_DefaultHints).ToArray();
            }
            else
            {
                m_Hints = inputHints.ToArray();
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
            try
            {
                return Create(type, new RandomizerChainer(m_Faker, m_Gen, Create));
            }
            catch (InsufficientExecutionStackException)
            {
                throw new InsufficientExecutionStackException(
                    $"Ran into infinite generation trying to randomize type '{type.Name}'.");
            }
        }

        /// <summary>Creates a randomized instance.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="chainer">Handles callback behavior for child values.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        private object Create(Type type, RandomizerChainer chainer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            (bool, object) result = m_Hints
                .Select(h => h.TryCreate(type, chainer))
                .FirstOrDefault(r => r.Item1);

            if (!result.Equals(default))
            {
                return result.Item2;
            }
            else
            {
                throw new NotSupportedException(
                    $"Type '{type.FullName}' not supported by the randomizer. " +
                    "Create a hint to generate the type and pass it to the randomizer.");
            }
        }

        /// <summary>
        ///     Constructs the parameters for a method.
        ///     Randomizes types by default.
        ///     Earlier fakes will be used to construct later types if possible.
        ///     Those types will be additionally populated with random fakes.
        /// </summary>
        /// <param name="method">Method to create parameters for.</param>
        /// <returns>Parameter arguments in order.</returns>
        public object[] CreateFor(MethodBase method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            Stack<object> args = new Stack<object>(method.GetParameters().Length);

            foreach (ParameterInfo param in method.GetParameters())
            {
                args.Push(Inject(param.ParameterType, args.Where(a => a is Fake).ToArray()));
            }

            return args.Reverse().ToArray();
        }

        /// <summary>Creates an instance using the values or random data as needed.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="values">Values to inject into the instance.</param>
        /// <returns>The created instance.</returns>
        public T Inject<T>(params object[] values)
        {
            return (T)Inject(typeof(T), values);
        }

        /// <summary>Creates an instance using the values or random data as needed.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="values">Values to inject into the instance.</param>
        /// <returns>The created instance.</returns>
        public object Inject(Type type, params object[] values)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            IDictionary<Type, object> data = (values ?? Array.Empty<object>())
                .Where(v => v != null)
                .Select(v => (v is Fake fake) ? fake.Dummy : v)
                .GroupBy(v => v.GetType())
                .ToDictionary(g => g.Key, g => g.Last());

            // Finds the contructor with the most matches then by fewest parameters.
            ConstructorInfo maker = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .GroupBy(c => c.GetParameters().Count(p => data.Keys.Any(t => t.Inherits(p.ParameterType))))
                .Where(g => g.Key > 0)
                .OrderByDescending(g => g.Key)
                .FirstOrDefault()
                ?.OrderBy(c => c.GetParameters())
                .FirstOrDefault();

            if (maker != null && !type.Inherits<Fake>())
            {
                ParameterInfo[] info = maker.GetParameters();
                object[] args = new object[info.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    Type key = data.Keys.FirstOrDefault(t => t.Inherits(info[i].ParameterType));
                    if (key != null)
                    {
                        args[i] = data[key];
                        data.Remove(key);
                    }
                    else
                    {
                        args[i] = Create(info[i].ParameterType);
                    }
                }
                return maker.Invoke(args);
            }
            else
            {
                return Create(type);
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
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return new Randomizer(duplicator.Copy(m_Faker),
                duplicator.Copy(m_Gen), false, duplicator.Copy(m_Hints).ToArray());
        }
    }
}
