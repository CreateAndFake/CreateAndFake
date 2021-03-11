using System;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls for comparables.</summary>
    public abstract class AssertComparableBase<T> : AssertObjectBase<T> where T : AssertComparableBase<T>
    {
        /// <summary>Value to check.</summary>
        protected IComparable Value { get; }

        /// <summary>Initializes a new instance of the <see cref="AssertComparableBase{T}"/> class.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="value">Value to check.</param>
        protected AssertComparableBase(IRandom gen, IValuer valuer, IComparable value) : base(gen, valuer, value)
        {
            Value = value;
        }

        /// <summary>Verifies value is &gt; <paramref name="expected"/>.</summary>
        /// <exception cref="AssertException">If value is not &lt;= <paramref name="expected"/>.</exception>
        /// <inheritdoc cref="HandleMathCheck"/>
        public virtual AssertChainer<T> GreaterThan(IComparable expected, string details = null)
        {
            return HandleMathCheck(() => Value.CompareTo(expected) > 0, "greater than", expected, details);
        }

        /// <summary>Verifies value is &gt;= <paramref name="expected"/>.</summary>
        /// <exception cref="AssertException">If value is &lt; <paramref name="expected"/>.</exception>
        /// <inheritdoc cref="HandleMathCheck"/>
        public virtual AssertChainer<T> GreaterThanOrEqualTo(IComparable expected, string details = null)
        {
            return HandleMathCheck(() => Value.CompareTo(expected) >= 0, "greater than or equal to", expected, details);
        }

        /// <summary>Verifies value is &gt; <paramref name="expected"/> or equals by value.</summary>
        /// <exception cref="AssertException">If value is &lt; <paramref name="expected"/>.</exception>
        /// <inheritdoc cref="HandleMathCheck"/>
        public virtual AssertChainer<T> GreaterThanOrIs(IComparable expected, string details = null)
        {
            return Valuer.Equals(Value, expected)
                ? ToChainer()
                : GreaterThanOrEqualTo(expected, details);
        }

        /// <summary>Verifies value is &lt; <paramref name="expected"/>.</summary>
        /// <exception cref="AssertException">If value is &gt;= <paramref name="expected"/>.</exception>
        /// <inheritdoc cref="HandleMathCheck"/>
        public virtual AssertChainer<T> LessThan(IComparable expected, string details = null)
        {
            return HandleMathCheck(() => Value.CompareTo(expected) < 0, "less than", expected, details);
        }

        /// <summary>Verifies value is &lt;= <paramref name="expected"/>.</summary>
        /// <exception cref="AssertException">If value is &gt; <paramref name="expected"/>.</exception>
        /// <inheritdoc cref="HandleMathCheck"/>
        public virtual AssertChainer<T> LessThanOrEqualTo(IComparable expected, string details = null)
        {
            return HandleMathCheck(() => Value.CompareTo(expected) <= 0, "less than or equal to", expected, details);
        }

        /// <summary>Verifies value is &lt; <paramref name="expected"/> or equals by value.</summary>
        /// <exception cref="AssertException">If value is &gt; <paramref name="expected"/>.</exception>
        /// <inheritdoc cref="HandleMathCheck"/>
        public virtual AssertChainer<T> LessThanOrIs(IComparable expected, string details = null)
        {
            return Valuer.Equals(Value, expected)
                ? ToChainer()
                : LessThanOrEqualTo(expected, details);
        }

        /// <summary>Verifies <paramref name="min"/> &lt;= value &lt;= <paramref name="max"/>.</summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Inclusive upper bound.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">
        ///     If value &lt; <paramref name="min"/> or &gt; <paramref name="max"/>.
        /// </exception>
        public virtual AssertChainer<T> InRange(IComparable min, IComparable max, string details = null)
        {
            if (Value == null)
            {
                throw new AssertException(
                    $"Value was null and not in range [{min}, {max}].", details, Gen.InitialSeed);
            }
            else if (min == null || max == null)
            {
                throw new AssertException(
                    $"Min {min} or max {max} was null and not valid for math comparison check.",
                    details, Gen.InitialSeed, Value.ToString());
            }
            else if (Value.CompareTo(min) < 0 || Value.CompareTo(max) > 0)
            {
                throw new AssertException(
                    $"Value was not in range [{min}, {max}].", details, Gen.InitialSeed, Value.ToString());
            }
            else
            {
                return ToChainer();
            }
        }

        /// <summary>Verifies value is matches the <paramref name="math"/>.</summary>
        /// <param name="math">Math used to check the value.</param>
        /// <param name="description">Math description to use for error message.</param>
        /// <param name="expected">Value to compare with.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If value does not match <paramref name="math"/>.</exception>
        private AssertChainer<T> HandleMathCheck(Func<bool> math,
            string description, IComparable expected, string details)
        {
            if (Value == null)
            {
                throw new AssertException(
                    $"Value was null and not {description} '{expected}'.", details, Gen.InitialSeed);
            }
            else if (expected == null)
            {
                throw new AssertException(
                    $"Expected was null and not valid for math comparison check.",
                    details, Gen.InitialSeed, Value.ToString());
            }
            else if (!math())
            {
                throw new AssertException(
                    $"Value was not {description} '{expected}'.", details, Gen.InitialSeed, Value.ToString());
            }
            else
            {
                return ToChainer();
            }
        }
    }
}
