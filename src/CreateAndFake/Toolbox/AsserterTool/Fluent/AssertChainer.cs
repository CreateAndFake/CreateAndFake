﻿using System.Collections;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <summary>Allows assertion calls to be chained fluently.</summary>
/// <typeparam name="T">Assertion base <c>Type</c> to chain.</typeparam>
/// <param name="chain">Assertion base instance to chain.</param>
/// <param name="gen">Core randomizer with a potential seed for logging.</param>
/// <param name="valuer">Handles comparisons for assertion checks.</param>
public sealed class AssertChainer<T>(T chain, IRandom gen, IValuer valuer)
{
    /// <summary>Includes another assertion on the instance to test.</summary>
    public T And { get; } = chain;

    /// <summary>Specifies a different instance to test.</summary>
    /// <param name="actual"><inheritdoc cref="AssertObjectBase{T}.Actual" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="actual"/> with.</returns>
    public AssertObject Also(object? actual)
    {
        return new AssertObject(gen, valuer, actual);
    }

    /// <param name="collection"><inheritdoc cref="AssertGroupBase{T}.Collection" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="collection"/> with.</returns>
    /// <inheritdoc cref="Also(object)"/>
    public AssertGroup Also(IEnumerable? collection)
    {
        return new AssertGroup(gen, valuer, collection);
    }

    /// <param name="text"><inheritdoc cref="AssertTextBase{T}.Text" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="text"/> with.</returns>
    /// <inheritdoc cref="Also(object)"/>
    public AssertText Also(string? text)
    {
        return new AssertText(Tools.Gen, Tools.Valuer, text);
    }

    /// <param name="value"><inheritdoc cref="AssertComparableBase{T}.Value" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="value"/> with.</returns>
    /// <inheritdoc cref="Also(object)"/>
    public AssertComparable Also(IComparable? value)
    {
        return new AssertComparable(Tools.Gen, Tools.Valuer, value);
    }

    /// <param name="type"><inheritdoc cref="AssertTypeBase{T}.Type" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="type"/> with.</returns>
    /// <inheritdoc cref="Also(object)"/>
    public AssertType Also(Type? type)
    {
        return new AssertType(Tools.Gen, Tools.Valuer, type);
    }

    /// <param name="error"><inheritdoc cref="AssertErrorBase{T}.Error" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="error"/> with.</returns>
    /// <inheritdoc cref="Also(object)"/>
    public AssertError Also(Exception? error)
    {
        return new AssertError(Tools.Gen, Tools.Valuer, error);
    }

    /// <param name="behavior"><inheritdoc cref="AssertBehaviorBase{T}.Behavior" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
    /// <inheritdoc cref="Also(object)"/>
    public AssertBehavior Also(Action? behavior)
    {
        return new AssertBehavior(Tools.Gen, Tools.Valuer, behavior);
    }

    /// <typeparam name="TReturn">Return <c>Type</c> of <paramref name="behavior"/>.</typeparam>
    /// <param name="behavior"><inheritdoc cref="AssertBehaviorBase{T}.Behavior" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
    /// <inheritdoc cref="Also(object)"/>
    public AssertBehavior Also<TReturn>(Func<TReturn>? behavior)
    {
        return new AssertBehavior(Tools.Gen, Tools.Valuer, behavior);
    }

    /// <summary>Handles assertion calls for runtime <paramref name="behavior"/>.</summary>
    /// <typeparam name="TOrigin"><c>Type</c> of <paramref name="origin"/>.</typeparam>
    /// <param name="origin">Object with <paramref name="behavior"/> to test.</param>
    /// <param name="behavior"><c>Delegate</c> on <paramref name="origin"/> to test.</param>
    /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
    /// <remarks>Primarily useful for exception testing.</remarks>
    public AssertBehavior Also<TOrigin>(TOrigin origin, Action<TOrigin> behavior)
    {
        return Also(() => behavior.Invoke(origin));
    }

    /// <inheritdoc cref="Also{T}(T,Action{T})"/>
    public AssertBehavior Also<TOrigin>(TOrigin origin, Func<TOrigin, object> behavior)
    {
        return Also(() => behavior.Invoke(origin));
    }
}
