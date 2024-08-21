﻿using System.Collections;
using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFake.Fluent;

/// <summary>Provides fluent assertions.</summary>
public static class AssertExtensions
{
    /// <inheritdoc cref="AssertObject"/>
    /// <param name="actual"><inheritdoc cref="AssertObjectBase{T}.Actual" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="actual"/> with.</returns>
    public static AssertObject Assert(this object? actual)
    {
        return new AssertObject(Tools.Gen, Tools.Valuer, actual);
    }

    /// <inheritdoc cref="AssertGroup"/>
    /// <param name="collection"><inheritdoc cref="AssertGroupBase{T}.Collection" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="collection"/> with.</returns>
    public static AssertGroup Assert(this IEnumerable? collection)
    {
        return new AssertGroup(Tools.Gen, Tools.Valuer, collection);
    }

    /// <inheritdoc cref="AssertText"/>
    /// <param name="text"><inheritdoc cref="AssertTextBase{T}.Text" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="text"/> with.</returns>
    public static AssertText Assert(this string? text)
    {
        return new AssertText(Tools.Gen, Tools.Valuer, text);
    }

    /// <inheritdoc cref="AssertComparable"/>
    /// <param name="value"><inheritdoc cref="AssertComparableBase{T}.Value" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="value"/> with.</returns>
    public static AssertComparable Assert(this IComparable? value)
    {
        return new AssertComparable(Tools.Gen, Tools.Valuer, value);
    }

    /// <inheritdoc cref="AssertBehavior"/>
    /// <param name="behavior"><inheritdoc cref="AssertBehaviorBase{T}.Behavior" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
    public static AssertBehavior Assert(this Action? behavior)
    {
        return new AssertBehavior(Tools.Gen, Tools.Valuer, behavior);
    }

    /// <inheritdoc cref="AssertBehavior"/>
    /// <typeparam name="T">Return <c>Type</c> of <paramref name="behavior"/>.</typeparam>
    /// <param name="behavior"><inheritdoc cref="AssertBehaviorBase{T}.Behavior" path="/summary"/></param>
    /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
    public static AssertBehavior Assert<T>(this Func<T>? behavior)
    {
        return new AssertBehavior(Tools.Gen, Tools.Valuer, behavior);
    }

    /// <summary>Handles assertion calls for runtime <paramref name="behavior"/>.</summary>
    /// <typeparam name="T"><c>Type</c> of <paramref name="origin"/>.</typeparam>
    /// <param name="origin">Object with <paramref name="behavior"/> to test.</param>
    /// <param name="behavior"><c>Delegate</c> on <paramref name="origin"/> to test.</param>
    /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
    /// <remarks>Primarily useful for exception testing.</remarks>
    public static AssertBehavior Assert<T>(this T origin, Action<T> behavior)
    {
        return Assert(() => behavior.Invoke(origin));
    }

    /// <inheritdoc cref="Assert{T}(T,Action{T})"/>
    public static AssertBehavior Assert<T>(this T origin, Func<T, object> behavior)
    {
        return Assert(() => behavior.Invoke(origin));
    }
}
