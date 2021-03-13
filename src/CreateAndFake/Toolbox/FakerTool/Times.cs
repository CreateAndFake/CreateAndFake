using System;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Tracks number of calls.</summary>
    public sealed class Times : IEquatable<Times>, IDeepCloneable
    {
        /// <summary>Expected bounds.</summary>
        private readonly int _min, _max;

        /// <summary>Initializes a new instance of the <see cref="Times"/> class.</summary>
        /// <param name="count">Upper and lower bound.</param>
        /// <remarks>Sets the expected bounds to a single value.</remarks>
        private Times(int count) : this(count, count) { }

        /// <summary>Initializes a new instance of the <see cref="Times"/> class.</summary>
        /// <param name="min">Lower bound.</param>
        /// <param name="max">Upper bound.</param>
        private Times(int min, int max)
        {
            _min = min;
            _max = max;
        }

        /// <inheritdoc/>
        public IDeepCloneable DeepClone()
        {
            return new Times(_min, _max);
        }

        /// <summary>Checks if <paramref name="count"/> is in expected range.</summary>
        /// <param name="count">Count to verify.</param>
        /// <returns>True if <paramref name="count"/> in range; false otherwise.</returns>
        internal bool IsInRange(int count)
        {
            return _min <= count && count <= _max;
        }

        /// <summary>Compares to <paramref name="obj"/> by value.</summary>
        /// <param name="obj">Instance to compare with.</param>
        /// <returns>True if equal to <paramref name="obj"/> by value; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Times);
        }

        /// <summary>Compares to <paramref name="other"/> by value.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <returns>True if equal to <paramref name="other"/> by value; false otherwise.</returns>
        public bool Equals(Times other)
        {
            return other != null
                && _min == other._min
                && _max == other._max;
        }

        /// <summary>Computes an identifying hash code based upon value.</summary>
        /// <returns>The computed hash code.</returns>
        public override int GetHashCode()
        {
            return ValueComparer.Use.GetHashCode(_min, _max);
        }

        /// <summary>Converts this object to a string.</summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            string maxValue = (_max != int.MaxValue)
                ? $"{_max}"
                : "*";
            return (_min != _max)
                ? $"[{_min}-{maxValue}]"
                : maxValue;
        }

        /// <summary>Sets the expected bounds to a single value.</summary>
        /// <param name="count">Upper and lower bound.</param>
        public static implicit operator Times(int count)
        {
            return ToTimes(count);
        }

        /// <summary>Sets the expected bounds to a single value.</summary>
        /// <param name="count">Upper and lower bound.</param>
        public static Times ToTimes(int count)
        {
            return new Times(count);
        }

        /// <summary>Represents 0 allowed calls.</summary>
        public static Times Never { get; } = Exactly(0);

        /// <summary>Represents 1 allowed calls.</summary>
        public static Times Once { get; } = Exactly(1);

        /// <summary>Sets the expected bounds to exactly <paramref name="count"/>.</summary>
        /// <param name="count">Upper and lower bound.</param>
        /// <returns>Representation of the bounds.</returns>
        public static Times Exactly(int count)
        {
            return new Times(count);
        }

        /// <summary>Sets the bounds to <paramref name="min"/> and <paramref name="max"/>.</summary>
        /// <param name="min">Lower bound.</param>
        /// <param name="max">Upper bound.</param>
        /// <returns>Representation of the bounds.</returns>
        public static Times Between(int min, int max)
        {
            return new Times(min, max);
        }

        /// <summary>Sets the expected bounds to anything above <paramref name="count"/>.</summary>
        /// <param name="count">Lower bound.</param>
        /// <returns>Representation of the bounds.</returns>
        public static Times Min(int count)
        {
            return Between(count, int.MaxValue);
        }

        /// <summary>Sets the expected bounds to anything below <paramref name="count"/>.</summary>
        /// <param name="count">Upper bound.</param>
        /// <returns>Representation of the bounds.</returns>
        public static Times Max(int count)
        {
            return Between(0, count);
        }

        /// <summary>Sets the expected bounds to any number.</summary>
        /// <returns>Representation of the bounds.</returns>
        public static Times Any()
        {
            return Min(0);
        }
    }
}
