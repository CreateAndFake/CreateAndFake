using System;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.MutatorTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Fluent;

/// <summary>Provides cloning assertions.</summary>
public static class CreateExtensions
{
    /// <inheritdoc cref="IDuplicator.Copy{T}(T)"/>
    public static T CreateDeepClone<T>(this T source)
    {
        return Tools.Duplicator.Copy(source);
    }

    /// <inheritdoc cref="IMutator.Variant{T}(T, T[])"/>
    public static T CreateVariant<T>(this T source)
    {
        return Tools.Mutator.Variant(source);
    }

    /// <inheritdoc cref="IRandomizer.Create(Type, Func{object, bool})"/>
    public static object CreateRandomInstance(this Type type)
    {
        return Tools.Randomizer.Create(type);
    }
}
