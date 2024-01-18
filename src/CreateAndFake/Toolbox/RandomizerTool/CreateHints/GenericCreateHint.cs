using System;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Design.Content;
using CreateAndFake.Design.Randomization;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles generation of generic types for the randomizer.</summary>
public sealed class GenericCreateHint : CreateHint
{
    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (type.IsGenericTypeDefinition)
        {
            return (true, Create(type, randomizer));
        }
        else
        {
            return (false, null);
        }
    }

    /// <summary>Creates a random instance of the given type.</summary>
    /// <param name="type">Type to generate.</param>
    /// <param name="randomizer">Handles callback behavior for child values.</param>
    /// <returns>Created instance.</returns>
    private static object Create(Type type, RandomizerChainer randomizer)
    {
        return randomizer.Create(type.MakeGenericType(
            type.GetGenericArguments().Select(a => CreateArg(a, randomizer)).ToArray()), randomizer.Parent);
    }

    /// <summary>Creates a concrete arg type from the given generic arg.</summary>
    /// <param name="type">Generic arg to create.</param>
    /// <param name="randomizer">Handles callback behavior for child values.</param>
    /// <returns>Created arg type.</returns>
    internal static Type CreateArg(Type type, RandomizerChainer randomizer)
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

        Type[] constraints = type.GetGenericParameterConstraints();

        Limiter.Dozen.Repeat(() =>
        {
            while (!constraints.All(c => arg.Inherits(c))
                && (!newNeeded || arg.GetConstructor(Type.EmptyTypes) != null))
            {
                object constraint = randomizer.Create(randomizer.Gen.NextItem(constraints), randomizer.Parent);
                arg = constraint.GetType();
                Disposer.Cleanup(constraint);
            }
        }).Wait();

        return arg;
    }
}
