﻿using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Design.Content;
using CreateAndFake.Design.Randomization;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles randomizing generic types for <see cref="IRandomizer"/>.</summary>
public sealed class GenericCreateHint : CreateHint
{
    /// <inheritdoc/>
    protected internal override (bool, object?) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (type?.IsGenericTypeDefinition ?? false)
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
    private static object Create(Type type, RandomizerChainer randomizer)
    {
        return randomizer.Create(type.MakeGenericType(type
            .GetGenericArguments()
            .Select(a => CreateArg(a, type, randomizer))
            .ToArray()), type);
    }

    /// <summary>Creates a concrete arg type from the given generic arg.</summary>
    /// <param name="type">Generic arg to create.</param>
    /// <param name="parent">Base <c>Type</c> being created.</param>
    /// <param name="randomizer">Handles randomizing child values.</param>
    /// <returns>Created arg <c>Type</c>.</returns>
    internal static Type CreateArg(Type type, Type parent, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        bool newNeeded = type.GenericParameterAttributes.HasFlag(
            GenericParameterAttributes.DefaultConstructorConstraint);

        Type arg;
        if (type.GenericParameterAttributes.HasFlag(
            GenericParameterAttributes.NotNullableValueTypeConstraint))
        {
            arg = randomizer.Gen.NextItem(ValueRandom.ValueTypes);
        }
        else if (newNeeded)
        {
            arg = typeof(object);
        }
        else
        {
            arg = typeof(string);
        }

        Type[] constraints = type
            .GetGenericParameterConstraints()
            .Select(t => t.ContainsGenericParameters ? t.GetGenericTypeDefinition() : t)
            .ToArray();

        bool isValidArg()
        {
            return constraints.All(c => arg.Inherits(c) || (arg.IsValueType && c == typeof(ValueType)))
                && (!newNeeded || arg.GetConstructor(Type.EmptyTypes) != null || arg.IsValueType);
        }

        if (!isValidArg())
        {
            Limiter.Few.Retry(
                $"Creating generic arguments of type '{type}' for type '{parent}' [Retry]",
                () => Limiter.Few.StallUntil($"Trying arguments of type '{type}' for type '{parent}' [Stall]", () =>
                {
                    arg = CreateArgViaConstraint(constraints, parent, randomizer);
                }, isValidArg).Wait()).Wait();
        }

        return arg;
    }

    /// <summary>Creates an arg type from the given constraints.</summary>
    /// <param name="constraints">Constraints limiting the arg type.</param>
    /// <param name="parent">Base <c>Type</c> being created.</param>
    /// <param name="randomizer">Handles randomizing child values.</param>
    /// <returns>Created arg <c>Type</c>.</returns>
    private static Type CreateArgViaConstraint(Type[] constraints, Type parent, RandomizerChainer randomizer)
    {
        Type constraint = randomizer.Gen.NextItem(constraints);
        if (parent == constraint)
        {
            return randomizer.Gen.NextItemOrDefault(parent.FindLoadedSubclasses())
                ?? throw new InvalidOperationException(
                    $"Cannot create '{parent}' due to self-reference and no visible subclasses.");
        }
        else
        {
            object sample = randomizer.Create(constraint);
            Type result = sample.GetType();
            Disposer.Cleanup(sample);
            return result;
        }
    }
}
