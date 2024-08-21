﻿using System.Reflection;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.FakerTool;

/// <inheritdoc cref="IFaker"/>
/// <param name="valuer">Handles comparisons.</param>
public sealed class Faker(IValuer valuer) : IFaker
{
    /// <inheritdoc/>
    public bool Supports<T>()
    {
        return Subclasser.Supports<T>();
    }

    /// <inheritdoc/>
    public bool Supports(Type type)
    {
        return Subclasser.Supports(type);
    }

    /// <inheritdoc/>
    public Fake<T> Mock<T>(params Type[] interfaces)
    {
        IFaked provider = Subclasser.Create(typeof(T), interfaces);
        provider.FakeMeta.Valuer = valuer;
        return new Fake<T>(provider);
    }

    /// <inheritdoc/>
    public Fake Mock(Type parent, params Type[] interfaces)
    {
        IFaked provider = Subclasser.Create(parent, interfaces);
        provider.FakeMeta.Valuer = valuer;
        return new Fake(provider);
    }

    /// <inheritdoc/>
    public Fake<T> Stub<T>(params Type[] interfaces)
    {
        Fake<T> fake = Mock<T>(interfaces);
        fake.ThrowByDefault = false;
        return fake;
    }

    /// <inheritdoc/>
    public Fake Stub(Type parent, params Type[] interfaces)
    {
        Fake fake = Mock(parent, interfaces);
        fake.ThrowByDefault = false;
        return fake;
    }

    /// <inheritdoc/>
    public Injected<T> InjectMocks<T>(params object[] values)
    {
        return Inject<T>(values ?? [], (Type t) => Mock(t));
    }

    /// <inheritdoc/>
    public Injected<T> InjectStubs<T>(params object[] values)
    {
        return Inject<T>(values ?? [], (Type t) => Stub(t));
    }

    /// <summary>Creates an instance injected with fakes.</summary>
    /// <typeparam name="T">Instance to be created.</typeparam>
    /// <param name="values">Values to inject instead where possible.</param>
    /// <param name="subclasser">Fake creation method to use.</param>
    /// <returns>The created instance with its fakes.</returns>
    private Injected<T> Inject<T>(object[] values, Func<Type, Fake> subclasser)
    {
        Type[] startingTypes = values
            .Where(v => v != null)
            .Select(v => (v is Fake fake) ? fake.Dummy : v)
            .Select(v => v.GetType())
            .ToArray();

        ConstructorInfo? maker = FindBestConstructor<T>(startingTypes);
        if (maker != null)
        {
            object?[] args = CreateInjectArgs(maker, values, subclasser);

            return new Injected<T>((T)maker.Invoke(args
                .Select(v => (v is Fake fake) ? fake.Dummy : v)
                .ToArray()), args.OfType<Fake>());
        }
        else
        {
            throw new InvalidOperationException($"No constructors found on type '{typeof(T).Name}'.");
        }
    }

    /// <summary>Creates the args to inject an instance with.</summary>
    /// <param name="maker">Constructor to use.</param>
    ///  <param name="values">Values to inject instead where possible.</param>
    /// <param name="subclasser">Fake creation method to use.</param>
    /// <returns>The created args to inject an instance with.</returns>
    private object?[] CreateInjectArgs(ConstructorInfo maker, object[] values, Func<Type, Fake> subclasser)
    {
        List<Tuple<Type, object>> data = values
            .Where(v => v != null)
            .Select(v => Tuple.Create((v is Fake fake) ? fake.Dummy.GetType() : v.GetType(), v))
            .ToList();

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
            else if (Supports(info[i].ParameterType))
            {
                args[i] = subclasser.Invoke(info[i].ParameterType);
            }
            else
            {
                args[i] = null;
            }
        }
        return args;
    }

    /// <summary>Finds the constructor with the most matches then by most parameters.</summary>
    /// <typeparam name="T">Type to search.</typeparam>
    /// <param name="startingTypes">Argument types to search on.</param>
    /// <returns>The constructor best fitted to the types.</returns>
    private static ConstructorInfo? FindBestConstructor<T>(Type[] startingTypes)
    {
        return typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .GroupBy(c => c.GetParameters().Count(p => startingTypes.Any(t => t.Inherits(p.ParameterType))))
            .OrderByDescending(g => g.Key)
            .FirstOrDefault()
            ?.OrderByDescending(c => c.GetParameters())
            .FirstOrDefault();
    }
}
