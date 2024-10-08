﻿using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.MutatorTool;

/// <inheritdoc cref="IMutator"/>
/// <param name="randomizer"><inheritdoc cref="_randomizer" path="/summary"/></param>
/// <param name="valuer"><inheritdoc cref="_valuer" path="/summary"/></param>
/// <param name="limiter"><inheritdoc cref="_limiter" path="/summary"/></param>
public sealed class Mutator(IRandomizer randomizer, IValuer valuer, Limiter limiter) : IMutator
{
    /// <summary>Handles randomization.</summary>
    private readonly IRandomizer _randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));

    /// <summary>Ensures object variance.</summary>
    private readonly IValuer _valuer = valuer ?? throw new ArgumentNullException(nameof(valuer));

    /// <summary>Limits attempts at creating variants.</summary>
    private readonly Limiter _limiter = limiter ?? throw new ArgumentNullException(nameof(limiter));

    /// <inheritdoc/>
    public T Variant<T>(T instance, params T?[]? extraInstances)
    {
        return (T)Variant(typeof(T), instance, extraInstances?.Cast<object>().ToArray());
    }

    /// <inheritdoc/>
    public object Variant(Type type, object? instance, params object?[]? extraInstances)
    {
        IEnumerable<object?> values = (extraInstances ?? Enumerable.Empty<object?>()).Prepend(instance);
        try
        {
            return _limiter.StallUntil(
                $"Create variant of type '{type}'",
                () => _randomizer.Create(type),
                result =>
                {
                    if (values.All(o => !_valuer.Equals(result, o)))
                    {
                        return true;
                    }
                    else
                    {
                        Disposer.Cleanup(result);
                        return false;
                    }
                }).Result.Last();
        }
        catch (AggregateException e)
        {
            throw new TimeoutException($"Could not create different instance of type '{type}'.", e);
        }
    }

    /// <inheritdoc/>
    public T Unique<T>(T instance, params T?[]? extraInstances)
    {
        return (T)Unique(typeof(T), instance, extraInstances);
    }

    /// <inheritdoc/>
    public object Unique(Type type, object? instance, params object?[]? extraInstances)
    {
        ContentMap[] maps = (extraInstances ?? [])
            .Prepend(instance)
            .Where(e => e != null)
            .Select(ContentMap.Extract)
            .ToArray();

        try
        {
            return _limiter.StallUntil(
                $"Create unique of type '{type}'",
                () => _randomizer.Create(type),
                result =>
                {
                    if (!ContentMap.Extract(result).HasSharedContent(valuer, maps))
                    {
                        return true;
                    }
                    else
                    {
                        Disposer.Cleanup(result);
                        return false;
                    }
                }).Result.Last();
        }
        catch (AggregateException e)
        {
            throw new TimeoutException($"Could not create unique instance of type '{type}'.", e);
        }
    }

    /// <inheritdoc/>
    public bool Modify(object? instance)
    {
        if (instance == null)
        {
            return false;
        }

        bool modified = false;

        Type type = instance.GetType();
        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            field.SetValue(instance, Variant(field.FieldType, field.GetValue(instance)));
            modified = true;
        }
        foreach (PropertyInfo property in type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanWrite && p.CanRead)
            .Where(p => p.GetGetMethod() != null)
            .Where(p => p.GetSetMethod() != null))
        {
            property.SetValue(instance, Variant(property.PropertyType, property.GetValue(instance)));
            modified = true;
        }

        return modified;
    }
}
