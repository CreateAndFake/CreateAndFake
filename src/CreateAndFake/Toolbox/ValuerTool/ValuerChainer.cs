using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Provides a callback into the valuer to create child values.</summary>
    public sealed class ValuerChainer : IValuer
    {
        /// <summary>History of comparisons to match up references.</summary>
        private readonly ICollection<(int, int)> m_CompareHistory;

        /// <summary>History of hashes to match up references.</summary>
        private readonly ICollection<int> m_HashHistory;

        /// <summary>Callback to valuer to handle child values.</summary>
        private readonly Func<object, object, ICollection<(int, int)>, IEnumerable<Difference>> m_Comparer;

        /// <summary>Callback to valuer to handle child values.</summary>
        private readonly Func<object, ICollection<int>, int> m_Hasher;

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="compareHistory">History of comparisons to match up references.</param>
        /// <param name="comparer">Callback to the valuer to handle child values.</param>
        public ValuerChainer(ICollection<(int, int)> compareHistory,
            Func<object, object, ICollection<(int, int)>, IEnumerable<Difference>> comparer)
        {
            m_Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            m_CompareHistory = compareHistory ?? new HashSet<(int, int)>();
        }

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="hashHistory">History of hashes to match up references.</param>
        /// <param name="hasher">Callback to the valuer to handle child values.</param>
        public ValuerChainer(ICollection<int> hashHistory, Func<object, ICollection<int>, int> hasher)
        {
            m_Hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            m_HashHistory = hashHistory ?? new HashSet<int>();
        }

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="compareHistory">History of comparisons to match up references.</param>
        /// <param name="comparer">Callback to the valuer to handle child values.</param>
        /// <param name="hashHistory">History of hashes to match up references.</param>
        /// <param name="hasher">Callback to the valuer to handle child values.</param>
        public ValuerChainer(ICollection<(int, int)> compareHistory,
            Func<object, object, ICollection<(int, int)>, IEnumerable<Difference>> comparer,
            ICollection<int> hashHistory, Func<object, ICollection<int>, int> hasher)
        {
            m_Hasher = hasher;
            m_HashHistory = hashHistory;
            m_Comparer = comparer;
            m_CompareHistory = compareHistory;
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <returns>Found differences.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        public IEnumerable<Difference> Compare(object expected, object actual)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            if (expected == null || actual == null)
            {
                return m_Comparer.Invoke(expected, actual, m_CompareHistory);
            }

            (int, int) refHash = (RuntimeHelpers.GetHashCode(expected), RuntimeHelpers.GetHashCode(actual));
            if (m_CompareHistory.Contains(refHash))
            {
                return Enumerable.Empty<Difference>();
            }

            m_CompareHistory.Add(refHash);
            return m_Comparer.Invoke(expected, actual, m_CompareHistory);
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
            if (item == null)
            {
                return m_Hasher.Invoke(item, m_HashHistory);
            }

            int refHash = RuntimeHelpers.GetHashCode(item);
            if (m_HashHistory.Contains(refHash))
            {
                return 0;
            }

            m_HashHistory.Add(refHash);
            return m_Hasher.Invoke(item, m_HashHistory);
        }

        /// <summary>Returns a hash code for the specified objects.</summary>
        /// <param name="items">Objects to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing an object.</exception>
        public int GetHashCode(params object[] items)
        {
            return GetHashCode((object)items);
        }
    }
}
