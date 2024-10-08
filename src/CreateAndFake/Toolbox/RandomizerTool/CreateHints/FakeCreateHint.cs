﻿using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles randomizing <see cref="Fake{T}"/> instances for <see cref="IRandomizer"/>.</summary>
public sealed class FakeCreateHint : CreateHint
{
    /// <summary>Possible action types to use.</summary>
    private static readonly Type[] _ActionTypes =
    [
        typeof(Action),
        typeof(Action<>),
        typeof(Action<,>),
        typeof(Action<,,>),
        typeof(Action<,,,>),
        typeof(Action<,,,,>),
        typeof(Action<,,,,,>),
        typeof(Action<,,,,,,>),
        typeof(Action<,,,,,,,>),
        typeof(Action<,,,,,,,,>),
        typeof(Action<,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,,,,>),
        typeof(Action<,,,,,,,,,,,,,,,>)
    ];

    /// <summary>Possible func types to use.</summary>
    private static readonly Type?[] _FuncTypes =
    [
        null,
        typeof(Func<>),
        typeof(Func<,>),
        typeof(Func<,,>),
        typeof(Func<,,,>),
        typeof(Func<,,,,>),
        typeof(Func<,,,,,>),
        typeof(Func<,,,,,,>),
        typeof(Func<,,,,,,,>),
        typeof(Func<,,,,,,,,>),
        typeof(Func<,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,,,,,,,>)
    ];

    /// <inheritdoc/>
    protected internal override (bool, object?) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (type.Inherits(typeof(Fake<>)))
        {
            return (true, Create(type, randomizer));
        }
        else
        {
            return (false, null);
        }
    }

    /// <returns>The randomized instance.</returns>
    /// <inheritdoc cref="CreateHint.TryCreate"/>
    private static Fake Create(Type type, RandomizerChainer randomizer)
    {
        Type target = type.GetGenericArguments().Single();

        Fake mock = randomizer.Stub(target);
        mock.Dummy.FakeMeta.Identifier = randomizer.Create<int>();

        // Generic returns have to just use stub behavior.
        foreach (MethodInfo method in target
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.IsAbstract || (m.IsVirtual && !m.IsFinal))
            .Where(m => !m.IsPrivate)
            .Where(m => !m.ReturnType.IsGenericParameter)
            .Where(m => !m.ReturnType.ContainsGenericParameters)
            .Where(m => m.Name != "Finalize"))
        {
            Type[] generics = method.IsGenericMethod
                ? method.GetGenericArguments().Select(a => typeof(AnyGeneric)).ToArray()
                : [];

            mock.Setup(method.Name, generics,
                method.GetParameters().Select(a => SetupMatch(a.ParameterType)).ToArray(),
                MakeBehavior(method, randomizer));
        }

        return (Fake)type.GetConstructor([typeof(Fake)])!.Invoke([mock]);
    }

    /// <summary>Sets up the random fake behavior for the method.</summary>
    /// <param name="method">Method to fake.</param>
    /// <param name="randomizer">Handles randomizing child values.</param>
    /// <returns>Behavior for the fake.</returns>
    private static Behavior MakeBehavior(MethodInfo method, RandomizerChainer randomizer)
    {
        Type[] args = method.GetParameters().Select(p => SetupArg(p.ParameterType)).ToArray();

        if (method.ReturnType != typeof(void))
        {
            Type[] withOut = [.. args, .. new[] { method.ReturnType }];

            return (Behavior)typeof(Behavior<>)
                .MakeGenericType(method.ReturnType)
                .GetConstructor([typeof(Delegate), typeof(Times)])!
                .Invoke([randomizer.Create(_FuncTypes[withOut.Length]!.MakeGenericType(withOut)), Times.Any()]);
        }
        else if (args.Length != 0)
        {
            return new Behavior<VoidType>((Delegate)randomizer
                .Create(_ActionTypes[args.Length].MakeGenericType(args))!, Times.Any());
        }
        else
        {
            return Behavior.None(Times.Any());
        }
    }

    /// <summary>Sets up arg types for the fake behavior.</summary>
    /// <param name="type"><c>Type</c> of the method to convert.</param>
    /// <returns>Type to use for the fake behavior delegate.</returns>
    private static Type SetupArg(Type type)
    {
        if (type.IsByRef)
        {
            return typeof(IOutRef);
        }
        else if (type.IsGenericParameter)
        {
            return typeof(object);
        }
        else
        {
            return type;
        }
    }

    /// <summary>Sets up the arg matcher for a parameter.</summary>
    /// <param name="type">Parameter <c>Type</c> to allow.</param>
    /// <returns>Arg to use for setting up the mock.</returns>
    private static object SetupMatch(Type type)
    {
        if (type.IsByRef)
        {
            return Arg.LambdaAny<IOutRef>();
        }
        else if (type.IsGenericParameter)
        {
            return Arg.LambdaAny<object>();
        }
        else
        {
            return typeof(Arg)
                .GetMethod(nameof(Arg.LambdaAny), BindingFlags.Static | BindingFlags.Public)!
                .MakeGenericMethod(type)
                .Invoke(null, [])!;
        }
    }
}
