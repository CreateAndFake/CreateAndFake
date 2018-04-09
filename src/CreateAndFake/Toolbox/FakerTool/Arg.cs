using System;
using System.Diagnostics.CodeAnalysis;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Provides the ability to match arguments based upon conditions.</summary>
    public sealed class Arg : IDeepCloneable
    {
        /// <summary>Condition to compare with.</summary>
        private readonly Func<object, bool> m_Matcher;

        /// <summary>Creates a matcher arg.</summary>
        /// <param name="matcher">Condition to compare with.</param>
        private Arg(Func<object, bool> matcher)
        {
            m_Matcher = matcher;
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <returns>Clone that is equal in value to the current instance.</returns>
        public IDeepCloneable DeepClone()
        {
            return new Arg(m_Matcher);
        }

        /// <summary>Determines if the condition is matched.</summary>
        /// <param name="arg">Argument to compare with.</param>
        /// <returns>True if matched; false otherwise.</returns>
        internal bool Matches(object arg)
        {
            return m_Matcher(arg);
        }

        /// <summary>Matches any instance of the given type.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <returns>Default instance of T for the fake setup.</returns>
        public static T Any<T>()
        {
            return default;
        }

        /// <summary>Matches any reference of the given type.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <returns>Container for the reference.</returns>
        public static OutRef<T> AnyRef<T>()
        {
            return new OutRef<T>();
        }

        /// <summary>Matches any instance but null of the given type.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <returns>Default instance of T for the fake setup.</returns>
        public static T NotNull<T>() where T : class
        {
            return default;
        }

        /// <summary>Matches any instance that fulfills the given condition.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="condition">Condition to verify.</param>
        /// <returns>Default instance of T for the fake setup.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters",
            Justification = "Taken and used through lambdas for setup replacement behavior.")]
        public static T Where<T>(Func<T, bool> condition)
        {
            return default;
        }

        /// <summary>Matches any reference that fulfills the given condition.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="condition">Condition to verify.</param>
        /// <returns>Container for the reference.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters",
            Justification = "Taken and used through lambdas for setup replacement behavior.")]
        public static OutRef<T> WhereRef<T>(Func<T, bool> condition)
        {
            return new OutRef<T>();
        }

        /// <summary>Matches any instance of the given type.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <returns>Arg for the fake provider to match with.</returns>
        public static Arg LambdaAny<T>()
        {
            return new Arg(o => o is T || o == null);
        }

        /// <summary>Matches any reference of the given type.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <returns>Arg for the fake provider to match with.</returns>
        public static Arg LambdaAnyRef<T>()
        {
            return LambdaWhere<OutRef<T>>(null);
        }

        /// <summary>Matches any instance but null of the given type.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <returns>Arg for the fake provider to match with.</returns>
        public static Arg LambdaNotNull<T>() where T : class
        {
            return LambdaWhere<T>(o => o != null);
        }

        /// <summary>Matches any instance that fulfills the given condition.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="condition">Condition to verify.</param>
        /// <returns>Arg for the fake provider to match with.</returns>
        public static Arg LambdaWhere<T>(Func<T, bool> condition)
        {
            return new Arg(o => o is T value && (condition?.Invoke(value) ?? true));
        }

        /// <summary>Matches any reference that fulfills the given condition.</summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="condition">Condition to verify.</param>
        /// <returns>Container for the reference.</returns>
        public static Arg LambdaWhereRef<T>(Func<T, bool> condition)
        {
            return LambdaWhere<OutRef<T>>(o => condition?.Invoke(o.Var) ?? true);
        }
    }
}
