using System;
using System.Linq;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls.</summary>
    public abstract class AssertObjectBase<T> where T : AssertObjectBase<T>
    {
        /// <summary>Handles comparisons.</summary>
        protected IValuer Valuer { get; }

        /// <summary>Object to compare with.</summary>
        protected object Actual { get; }

        /// <summary>Sets up the asserter capabilities.</summary>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="actual">Object to compare with.</param>
        /// <exception cref="ArgumentNullException">If given a null valuer.</exception>
        protected AssertObjectBase(IValuer valuer, object actual)
        {
            Valuer = valuer ?? throw new ArgumentNullException(nameof(valuer));

            Actual = actual;
        }

        /// <summary>Verifies two objects are equal by value.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public AssertChainer<T> Is(object expected, string details = null)
        {
            return ValuesEqual(expected, details);
        }

        /// <summary>Verifies two objects are unequal by value.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public AssertChainer<T> IsNot(object expected, string details = null)
        {
            return ValuesNotEqual(expected, details);
        }

        /// <summary>Verifies two objects are equal by reference.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public virtual AssertChainer<T> ReferenceEqual(object expected, string details = null)
        {
            if (!ReferenceEquals(expected, Actual))
            {
                throw new AssertException("References failed to equal.", details);
            }
            return ToChainer();
        }

        /// <summary>Verifies two objects are not equal by reference.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public virtual AssertChainer<T> ReferenceNotEqual(object expected, string details = null)
        {
            if (ReferenceEquals(expected, Actual))
            {
                throw new AssertException("References failed to not equal.", details);
            }
            return ToChainer();
        }

        /// <summary>Verifies two objects are equal by value.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public virtual AssertChainer<T> ValuesEqual(object expected, string details = null)
        {
            Difference[] differences = Valuer.Compare(expected, Actual).ToArray();
            if (differences.Length > 0)
            {
                throw new AssertException($"Value equality failed for type '{GetTypeName(expected)}'.",
                    details, string.Join<Difference>(Environment.NewLine, differences));
            }
            return ToChainer();
        }

        /// <summary>Verifies two objects are unequal by value.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public virtual AssertChainer<T> ValuesNotEqual(object expected, string details = null)
        {
            if (!Valuer.Compare(expected, Actual).Any())
            {
                throw new AssertException(
                    $"Value inequality failed for type '{GetTypeName(expected)}'.", details, expected?.ToString());
            }
            return ToChainer();
        }

        /// <summary>Find a type name to use for messages.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <returns>Found name to use.</returns>
        protected string GetTypeName(object expected)
        {
            return (expected ?? Actual)?.GetType().Name;
        }

        /// <summary>Converts the instance to a chainer.</summary>
        /// <returns>The created chainer.</returns>
        protected AssertChainer<T> ToChainer()
        {
            return new AssertChainer<T>((T)this);
        }
    }
}
