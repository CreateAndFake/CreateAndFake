using System;
using System.Collections;
using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFake.Fluent
{
    /// <summary>Provides fluent assertions.</summary>
    public static class AssertExtensions
    {
        /// <summary>Handles common test scenarios.</summary>
        /// <param name="actual">Object to test.</param>
        /// <returns>Asserter to test <paramref name="actual"/> with.</returns>
        public static AssertObject Assert(this object actual)
        {
            return new AssertObject(Tools.Gen, Tools.Valuer, actual);
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <param name="collection">Collection to test.</param>
        /// <returns>Asserter to test <paramref name="collection"/> with.</returns>
        public static AssertGroup Assert(this IEnumerable collection)
        {
            return new AssertGroup(Tools.Gen, Tools.Valuer, collection);
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <param name="text">Text to test.</param>
        /// <returns>Asserter to test <paramref name="text"/> with.</returns>
        public static AssertText Assert(this string text)
        {
            return new AssertText(Tools.Gen, Tools.Valuer, text);
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <param name="value">Value to test.</param>
        /// <returns>Asserter to test <paramref name="value"/> with.</returns>
        public static AssertComparable Assert(this IComparable value)
        {
            return new AssertComparable(Tools.Gen, Tools.Valuer, value);
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <param name="behavior">Delegate to test.</param>
        /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
        public static AssertBehavior Assert(this Action behavior)
        {
            return new AssertBehavior(Tools.Gen, Tools.Valuer, behavior);
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <typeparam name="T">Return type of <paramref name="behavior"/>.</typeparam>
        /// <param name="behavior">Delegate to test.</param>
        /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
        public static AssertBehavior Assert<T>(this Func<T> behavior)
        {
            return new AssertBehavior(Tools.Gen, Tools.Valuer, behavior);
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <typeparam name="T">Return type of <paramref name="behavior"/>.</typeparam>
        /// <param name="actual">Object with <paramref name="behavior"/> to test.</param>
        /// <param name="behavior">Delegate on <paramref name="actual"/> to test.</param>
        /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
        public static AssertBehavior Assert<T>(this T actual, Action<T> behavior)
        {
            return Assert(() => behavior.Invoke(actual));
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <typeparam name="T">Return type of <paramref name="behavior"/>.</typeparam>
        /// <param name="actual">Object with <paramref name="behavior"/> to test.</param>
        /// <param name="behavior">Delegate on <paramref name="actual"/> to test.</param>
        /// <returns>Asserter to test <paramref name="behavior"/> with.</returns>
        public static AssertBehavior Assert<T>(this T actual, Func<T, object> behavior)
        {
            return Assert(() => behavior.Invoke(actual));
        }
    }
}
