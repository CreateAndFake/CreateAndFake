using System;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Tracks number of calls.</summary>
    public sealed class Times : IEquatable<Times>, IDeepCloneable
    {
        /// <summary>Expected bounds.</summary>
        private readonly int m_Min, m_Max;

        /// <summary>Sets the expected bounds to a single value.</summary>
        /// <param name="count">Upper and lower bound.</param>
        private Times(int count) : this(count, count) { }

        /// <summary>Sets the bounds to the given values.</summary>
        /// <param name="min">Lower bound.</param>
        /// <param name="max">Upper bound.</param>
        private Times(int min, int max)
        {
            m_Min = min;
            m_Max = max;
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <returns>Clone that is equal in value to the current instance.</returns>
        public IDeepCloneable DeepClone()
        {
            return new Times(m_Min, m_Max);
        }

        /// <summary>Checks if a count is in expected range.</summary>
        /// <param name="count">Count to verify.</param>
        /// <returns>True if in range; false otherwise.</returns>
        internal bool IsInRange(int count)
        {
            return m_Min <= count && count <= m_Max;
        }

        /// <summary>Checks for value equality.</summary>
        /// <param name="obj">Instance to compare with.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Times);
        }

        /// <summary>Checks for value equality.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public bool Equals(Times other)
        {
            return other != null
                && m_Min == other.m_Min
                && m_Max == other.m_Max;
        }

        /// <returns>Hash code based upon value identifying the object.</returns>
        public override int GetHashCode()
        {
            return ValueComparer.Use.GetHashCode(m_Min, m_Max);
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            string maxValue = (m_Max != int.MaxValue)
                ? $"{m_Max}"
                : "*";
            return (m_Min != m_Max)
                ? $"[{m_Min}-{maxValue}]"
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

        /// <summary>Sets the expected bounds to a single value.</summary>
        /// <param name="count">Upper and lower bound.</param>
        /// <returns>Representation of the bounds.</returns>
        public static Times Exactly(int count) => new Times(count);

        /// <summary>Sets the bounds to the given values.</summary>
        /// <param name="min">Lower bound.</param>
        /// <param name="max">Upper bound.</param>
        /// <returns>Representation of the bounds.</returns>
        public static Times Between(int min, int max) => new Times(min, max);

        /// <summary>Sets the expected bounds to anything above the given value.</summary>
        /// <param name="count">Lower bound.</param>
        /// <returns>Representation of the bounds.</returns>
        public static Times Min(int count) => Between(count, int.MaxValue);

        /// <summary>Sets the expected bounds to anything below the given value.</summary>
        /// <param name="count">Upper bound.</param>
        /// <returns>Representation of the bounds.</returns>
        public static Times Max(int count) => Between(0, count);
    }
}
