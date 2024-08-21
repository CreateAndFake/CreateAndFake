using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Design.Content;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;

namespace CreateAndFake.Toolbox.RandomizerTool;

/// <inheritdoc cref="IRandomizer"/>
/// <param name="faker"><inheritdoc cref="_faker" path="/summary"/></param>
/// <param name="gen"><inheritdoc cref="_gen" path="/summary"/></param>
/// <param name="limiter"><inheritdoc cref="_limiter" path="/summary"/></param>
/// <param name="includeDefaultHints">If the default set of hints should be added.</param>
/// <param name="hints"><inheritdoc cref="_hints" path="/summary"/></param>
public sealed class Randomizer(
    IFaker faker,
    IRandom gen,
    Limiter limiter,
    bool includeDefaultHints = true,
    params CreateHint[] hints)
    : IRandomizer, IDuplicatable
{
    /// <summary>Default set of hints to use for randomization.</summary>
    private static readonly CreateHint[] _DefaultHints =
    [
        new ValueCreateHint(),
        new EnumCreateHint(),
        new GenericCreateHint(),
        new AsyncCollectionCreateHint(),
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
    ];

    /// <summary>Provides stubs.</summary>
    private readonly IFaker _faker = faker ?? throw new ArgumentNullException(nameof(faker));

    /// <summary>Value generator used for base randomization.</summary>
    private readonly IRandom _gen = gen ?? throw new ArgumentNullException(nameof(gen));

    /// <summary>Limits attempts at matching conditions.</summary>
    private readonly Limiter _limiter = limiter ?? throw new ArgumentNullException(nameof(limiter));

    /// <summary>Generators used to randomize specific types.</summary>
    private readonly List<CreateHint> _hints = (hints ?? Enumerable.Empty<CreateHint>())
        .Concat(includeDefaultHints ? _DefaultHints : [])
        .ToList();

    /// <inheritdoc/>
    public T Create<T>(Func<T, bool>? condition = null)
    {
        return (T)Create(typeof(T), o => condition?.Invoke((T)o) ?? true);
    }

    /// <inheritdoc/>
    public object Create(Type type, Func<object, bool>? condition = null)
    {
        try
        {
            return _limiter.StallUntil(
                $"Trying to create instance of '{type}'",
                () => Create(type, new RandomizerChainer(_faker, _gen, Create)),
                result =>
                {
                    if (condition?.Invoke(result!) ?? true)
                    {
                        return true;
                    }
                    else
                    {
                        Disposer.Cleanup(result);
                        return false;
                    }
                }).Result.Last()!;
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
    }

    /// <param name="type">Type to create.</param>
    /// <param name="chainer">Handles callback behavior for child values.</param>
    /// <inheritdoc cref="Create(Type,Func{object,bool})"/>
    private object Create(Type type, RandomizerChainer chainer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        (bool, object?) result = _hints
            .Select(h => h.TryCreate(type, chainer))
            .FirstOrDefault(r => r.Item1);

        if (!result.Equals(default))
        {
            return result.Item2!;
        }
        else
        {
            throw new NotSupportedException(
                $"Type '{type.FullName}' not supported by the randomizer. " +
                "Create a hint to generate the type and pass it to the randomizer.");
        }
    }

    /// <inheritdoc/>
    public T CreateSized<T>(int count)
    {
        return (T)CreateSized(typeof(T), count);
    }

    /// <inheritdoc/>
    public object CreateSized(Type type, int count)
    {
        try
        {
            return CreateSized(type, count, new RandomizerChainer(_faker, _gen, Create));
        }
        catch (InsufficientExecutionStackException)
        {
            throw new InsufficientExecutionStackException(
                $"Ran into infinite generation trying to randomize type '{type}'.");
        }
        catch (Exception e) when (e is not NotSupportedException)
        {
            throw new InvalidOperationException(
                $"Encountered issue creating instance of type '{type}'.", e);
        }
    }

    /// <param name="type">Type to create.</param>
    /// <param name="count">Number of items to generate.</param>
    /// <param name="chainer">Handles callback behavior for child values.</param>
    /// <inheritdoc cref="CreateSized(Type,int)"/>
    private object CreateSized(Type type, int count, RandomizerChainer chainer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        (bool, object?) result = _hints
            .OfType<CreateCollectionHint>()
            .Select(h => h.TryCreate(type, count, chainer))
            .FirstOrDefault(r => r.Item1);

        if (!result.Equals(default))
        {
            return result.Item2!;
        }
        else
        {
            throw new NotSupportedException(
                $"Collection type '{type.FullName}' not supported by the randomizer. " +
                "Create a collection hint to generate the type and pass it to the randomizer.");
        }
    }

    /// <inheritdoc/>
    public MethodCallWrapper CreateFor(MethodBase method, params object?[]? values)
    {
        ArgumentGuard.ThrowIfNull(method, nameof(method));

        List<Tuple<Type, object>> data = (values ?? [])
            .Where(v => v != null)
            .Select(v => (v is Fake fake) ? fake.Dummy : v)
            .Where(v => v != null)
            .Select(v => Tuple.Create(v!.GetType(), v))
            .ToList();

        List<Tuple<string, object?>> args = new(method.GetParameters().Length);

        foreach (ParameterInfo param in method.GetParameters())
        {
            args.Add(Tuple.Create(param.Name ?? $"{args.Count}", ExtractArg(param, data, args)));
        }

        return new MethodCallWrapper(method, args);
    }

    /// <summary>Randomizes an instance to fill a parameter.</summary>
    /// <param name="param">Parameter to fill.</param>
    /// <param name="data">Canned data to prefer.</param>
    /// <param name="args">Already created parameter data.</param>
    /// <returns>The created arg to fill the parameter with.</returns>
    private object? ExtractArg(ParameterInfo param, List<Tuple<Type, object>> data, List<Tuple<string, object?>> args)
    {
        Tuple<Type, object> match = data.FirstOrDefault(t => t.Item1.Inherits(param.ParameterType))!;
        if (param.IsOut)
        {
            return null;
        }
        else if (param.GetCustomAttributes<FakeAttribute>().Any())
        {
            return ((Fake)Create(typeof(Fake<>).MakeGenericType(param.ParameterType))!).Dummy;
        }
        else if (param.GetCustomAttributes<StubAttribute>().Any())
        {
            return _faker.Stub(param.ParameterType).Dummy;
        }
        else if (param.GetCustomAttributes<SizeAttribute>().Any())
        {
            return CreateSized(param.ParameterType, param.GetCustomAttribute<SizeAttribute>()!.Count);
        }
        else if (match != default)
        {
            _ = data.Remove(match);
            return match.Item2;
        }
        else
        {
            return Inject(param.ParameterType, args
                .Select(a => a.Item2)
                .Where(a => a is Fake or IFaked)
                .Reverse()
                .ToArray());
        }
    }

    /// <inheritdoc/>
    public T Inject<T>(params object?[]? values)
    {
        return (T)Inject(typeof(T), values);
    }

    /// <inheritdoc/>
    public object Inject(Type type, params object?[]? values)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        List<Tuple<Type, object>> data = (values ?? [])
            .Where(v => v != null)
            .Select(v => (v is Fake fake) ? fake.Dummy : v)
            .Where(v => v != null)
            .Select(v => Tuple.Create(v!.GetType(), v))
            .ToList();

        ConstructorInfo? maker = FindConstructor(type, data, BindingFlags.Public)
            ?? FindConstructor(type, data, BindingFlags.NonPublic);

        if (maker != null && !type.Inherits<Fake>() && !type.Inherits(typeof(Injected<>)))
        {
            return maker.Invoke(CreateInjectArgs(maker, data));
        }
        else
        {
            return Create(type);
        }
    }

    /// <summary>Creates the args to inject an instance with.</summary>
    /// <param name="maker">Constructor to use.</param>
    /// <param name="data">Canned data to prefer.</param>
    /// <returns>The created args to inject an instance with.</returns>
    private object?[] CreateInjectArgs(ConstructorInfo maker, List<Tuple<Type, object>> data)
    {
        ParameterInfo[] info = maker.GetParameters();
        object?[] args = new object[info.Length];

        for (int i = 0; i < args.Length; i++)
        {
            Tuple<Type, object>? match = data.FirstOrDefault(t => t.Item1.Inherits(info[i].ParameterType));
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
        return args;
    }

    /// <summary>Finds the constructor with the most matches then by fewest parameters.</summary>
    /// <param name="type">Type to find a constructor for.</param>
    /// <param name="data">Injection data to use.</param>
    /// <param name="scope">Scope of constructors to find.</param>
    /// <returns>Constructor if found; null otherwise.</returns>
    private static ConstructorInfo? FindConstructor(Type type, List<Tuple<Type, object>> data, BindingFlags scope)
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
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        return new Randomizer(duplicator.Copy(_faker)!, duplicator.Copy(_gen)!,
            duplicator.Copy(_limiter)!, false, [.. duplicator.Copy(_hints)]);
    }

    /// <inheritdoc/>
    public void AddHint(CreateHint hint)
    {
        _hints.Insert(0, hint ?? throw new ArgumentNullException(nameof(hint)));
    }
}
