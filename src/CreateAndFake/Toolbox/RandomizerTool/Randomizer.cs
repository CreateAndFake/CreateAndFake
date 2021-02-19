using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <inheritdoc cref='IRandomizer'/>
    public sealed class Randomizer : IRandomizer, IDuplicatable
    {
        /// <summary>Default set of hints to use for randomization.</summary>
        private static readonly CreateHint[] _DefaultHints = new CreateHint[]
        {
            new ValueCreateHint(),
            new EnumCreateHint(),
            new GenericCreateHint(),
            new CollectionCreateHint(),
            new LegacyCollectionCreateHint(),
            new StringCreateHint(),
            new DelegateCreateHint(),
            new TaskCreateHint(),
            new CommonSystemCreateHint(),
            new InjectedCreateHint(),
            new FakeCreateHint(),
            new FakedCreateHint(),
            new ExceptionCreateHint(),
            new ObjectCreateHint()
        };

        /// <summary>Provides stubs.</summary>
        private readonly IFaker _faker;

        /// <summary>Generators used to randomize specific types.</summary>
        private readonly IList<CreateHint> _hints;

        /// <summary>Value generator used for base randomization.</summary>
        private readonly IRandom _gen;

        /// <summary>Limits attempts at matching conditions.</summary>
        private readonly Limiter _limiter;

        /// <summary>Sets up the randomizer capabilities.</summary>
        /// <param name="faker">Provides stubs.</param>
        /// <param name="gen">Value generator to use for base randomization.</param>
        /// <param name="includeDefaultHints">If the default set of hints should be added.</param>
        /// <param name="hints">Generators used to randomize specific types.</param>
        /// <param name="limiter">Limits attempts at matching conditions.</param>
        public Randomizer(IFaker faker, IRandom gen, Limiter limiter,
            bool includeDefaultHints = true, params CreateHint[] hints)
        {
            _faker = faker ?? throw new ArgumentNullException(nameof(faker));
            _gen = gen ?? throw new ArgumentNullException(nameof(gen));
            _limiter = limiter ?? throw new ArgumentNullException(nameof(limiter));

            IEnumerable<CreateHint> inputHints = hints ?? Enumerable.Empty<CreateHint>();
            _hints = (includeDefaultHints)
                ? inputHints.Concat(_DefaultHints).ToList()
                : inputHints.ToList();
        }

        /// <inheritdoc/>
        public T Create<T>(Func<T, bool> condition = null)
        {
            return (T)Create(typeof(T), o => condition?.Invoke((T)o) ?? true);
        }

        /// <inheritdoc/>
        public object Create(Type type, Func<object, bool> condition = null)
        {
            object result = default;
            try
            {
                _limiter.StallUntil(
                    () => result = Create(type, new RandomizerChainer(_faker, _gen, Create)),
                    () => condition?.Invoke(result) ?? true).Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException is InsufficientExecutionStackException)
                {
                    throw new InsufficientExecutionStackException(
                        $"Ran into infinite generation trying to randomize type '{type}'.");
                }
                else if (e.InnerException is TimeoutException)
                {
                    throw new TimeoutException(
                        $"Could not create instance of type '{type}' matching condition.", e);
                }
                else if (e.InnerException is NotSupportedException)
                {
                    throw e.InnerException;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Encountered issue creating instance of type '{type}'.", e);
                }
            }
            return result;
        }

        /// <summary>Creates a randomized instance.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="chainer">Handles callback behavior for child values.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        private object Create(Type type, RandomizerChainer chainer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            (bool, object) result = _hints
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

        /// <inheritdoc/>
        public MethodCallWrapper CreateFor(MethodBase method, params object[] values)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            List<Tuple<Type, object>> data = (values ?? Array.Empty<object>())
                .Where(v => v != null)
                .Select(v => (v is Fake fake) ? fake.Dummy : v)
                .Select(v => Tuple.Create(v.GetType(), v))
                .ToList();

            IList<Tuple<string, object>> args = new List<Tuple<string, object>>(method.GetParameters().Length);
            void AddArg(string name, object data)
            {
                args.Add(Tuple.Create(name ?? $"{args.Count}", data));
            }

            foreach (ParameterInfo param in method.GetParameters())
            {
                Tuple<Type, object> match = data.FirstOrDefault(t => t.Item1.Inherits(param.ParameterType));
                if (param.IsOut)
                {
                    AddArg(param.Name, null);
                }
                else if (match != default)
                {
                    AddArg(param.Name, match.Item2);
                    _ = data.Remove(match);
                }
                else
                {
                    AddArg(param.Name, Inject(param.ParameterType, args
                        .Select(a => a.Item2)
                        .Where(a => a is Fake)
                        .Reverse()
                        .ToArray()));
                }
            }

            return new MethodCallWrapper(method, args);
        }

        /// <inheritdoc/>
        public T Inject<T>(params object[] values)
        {
            return (T)Inject(typeof(T), values);
        }

        /// <inheritdoc/>
        public object Inject(Type type, params object[] values)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            List<Tuple<Type, object>> data = (values ?? Array.Empty<object>())
                .Where(v => v != null)
                .Select(v => (v is Fake fake) ? fake.Dummy : v)
                .Select(v => Tuple.Create(v.GetType(), v))
                .ToList();

            ConstructorInfo maker = FindConstructor(type, data, BindingFlags.Public)
                ?? FindConstructor(type, data, BindingFlags.NonPublic);

            if (maker != null && !type.Inherits<Fake>() && !type.Inherits(typeof(Injected<>)))
            {
                ParameterInfo[] info = maker.GetParameters();
                object[] args = new object[info.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    Tuple<Type, object> match = data.FirstOrDefault(t => t.Item1.Inherits(info[i].ParameterType));
                    if (match != default)
                    {
                        args[i] = match.Item2;
                        _ = data.Remove(match);
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

        /// <summary>Finds the contructor with the most matches then by fewest parameters.</summary>
        /// <param name="type">Type to find a constructor for.</param>
        /// <param name="data">Injection data to use.</param>
        /// <param name="scope">Scope of constructors to find.</param>
        /// <returns>Constructor if found; null otherwise.</returns>
        private static ConstructorInfo FindConstructor(Type type, List<Tuple<Type, object>> data, BindingFlags scope)
        {
            return type.GetConstructors(BindingFlags.Instance | scope)
                .GroupBy(c => c.GetParameters().Count(p => data.Any(t => t.Item1.Inherits(p.ParameterType))))
                .Where(g => g.Key > 0)
                .OrderByDescending(g => g.Key)
                .FirstOrDefault()
                ?.OrderBy(c => c.GetParameters())
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public IDuplicatable DeepClone(IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return new Randomizer(duplicator.Copy(_faker), duplicator.Copy(_gen),
                duplicator.Copy(_limiter), false, duplicator.Copy(_hints).ToArray());
        }

        /// <inheritdoc/>
        public void AddHint(CreateHint hint)
        {
            _hints.Insert(0, hint ?? throw new ArgumentNullException(nameof(hint)));
        }
    }
}
