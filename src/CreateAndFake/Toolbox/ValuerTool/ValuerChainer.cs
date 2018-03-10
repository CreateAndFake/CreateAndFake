using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Provides a callback into the valuer to create child values.</summary>
    public sealed class ValuerChainer
    {
        /// <summary>History of comparisons to match up references.</summary>
        private readonly ICollection<(int, int)> m_CompareHistory;

        /// <summary>History of hashes to match up references.</summary>
        private readonly IDictionary<int, object> m_HashHistory;

        /// <summary>Callback to valuer to handle child values.</summary>
        private readonly Func<object, object, ValuerChainer, IEnumerable<Difference>> m_Comparer;

        /// <summary>Callback to valuer to handle child values.</summary>
        private readonly Func<object, ValuerChainer, int> m_Hasher;

        /// <summary>Reference to the actual valuer.</summary>
        internal IValuer Valuer { get; }

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="valuer">Reference to the actual valuer.</param>
        /// <param name="hasher">Callback to the valuer to handle child values.</param>
        /// <param name="comparer">Callback to the valuer to handle child values.</param>
        public ValuerChainer(IValuer valuer, Func<object, ValuerChainer, int> hasher,
            Func<object, object, ValuerChainer, IEnumerable<Difference>> comparer)
        {
            Valuer = valuer ?? throw new ArgumentNullException(nameof(valuer));
            m_Hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            m_Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

            m_CompareHistory = new HashSet<(int, int)>();
            m_HashHistory = new Dictionary<int, object>();
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <returns>Found differences.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        public IEnumerable<Difference> Compare(object expected, object actual)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            if (!CanTrack(expected) || !CanTrack(actual))
            {
                return m_Comparer.Invoke(expected, actual, this);
            }

            (int, int) refHash = (RuntimeHelpers.GetHashCode(expected), RuntimeHelpers.GetHashCode(actual));
            if (m_CompareHistory.Contains(refHash))
            {
                return Enumerable.Empty<Difference>();
            }

            m_CompareHistory.Add(refHash);
            return m_Comparer.Invoke(expected, actual, this);
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if equal; false otherwise.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        public new bool Equals(object x, object y)
        {
            return !Compare(x, y).Any();
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing the object.</exception>
        public int GetHashCode(object item)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            if (!CanTrack(item))
            {
                return m_Hasher.Invoke(item, this);
            }

            int refHash = RuntimeHelpers.GetHashCode(item);
            if (m_HashHistory.TryGetValue(refHash, out object stored)
                && ReferenceEquals(item, stored))
            {
                return 0;
            }

            m_HashHistory[refHash] = item;
            return m_Hasher.Invoke(item, this);
        }

        /// <summary>Returns a hash code for the specified objects.</summary>
        /// <param name="items">Objects to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing an object.</exception>
        public int GetHashCode(params object[] items)
        {
            return GetHashCode((object)items);
        }

        /// <summary>If the object can be tracked in history.</summary>
        /// <param name="source">Item to check.</param>
        /// <returns>True if possible; false otherwise.</returns>
        private static bool CanTrack(object source)
        {
            return !(source == null || source is IFaked || source.GetType().IsValueType);
        }
    }
}
