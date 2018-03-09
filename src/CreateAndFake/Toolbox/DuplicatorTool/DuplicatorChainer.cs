using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CreateAndFake.Toolbox.DuplicatorTool
{
    /// <summary>Provides a callback into the duplicator to create child values.</summary>
    public sealed class DuplicatorChainer : IDuplicator
    {
        /// <summary>History of clones to match up references.</summary>
        private readonly IDictionary<int, object> m_History;

        /// <summary>Callback to the duplicator to handle child values.</summary>
        private readonly Func<object, IDictionary<int, object>, object> m_Duplicator;

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="history">History of clones to match up references.</param>
        /// <param name="duplicator">Callback to the duplicator to handle child values.</param>
        public DuplicatorChainer(IDictionary<int, object> history,
            Func<object, IDictionary<int, object>, object> duplicator)
        {
            m_Duplicator = duplicator ?? throw new ArgumentNullException(nameof(duplicator));
            m_History = history ?? new Dictionary<int, object>();
        }

        /// <summary>Adds created type to history.</summary>
        /// <param name="source">Object cloned.</param>
        /// <param name="clone">The clone.</param>
        public void AddToHistory(object source, object clone)
        {
            m_History.Add(RuntimeHelpers.GetHashCode(source), clone);
        }

        /// <summary>Deep clones an object.</summary>
        /// <typeparam name="T">Type being cloned.</typeparam>
        /// <param name="source">Object to clone.</param>
        /// <returns>The duplicate.</returns>
        /// <exception cref="NotSupportedException">If no hint supports cloning the object.</exception>
        public T Copy<T>(T source)
        {
            return (T)Copy((object)source);
        }

        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <returns>The duplicate.</returns>
        /// <exception cref="NotSupportedException">If no hint supports cloning the object.</exception>
        public object Copy(object source)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            if (source == null)
            {
                return m_Duplicator.Invoke(source, m_History);
            }

            int refHash = RuntimeHelpers.GetHashCode(source);
            if (m_History.TryGetValue(refHash, out object clone))
            {
                return clone;
            }

            object result = m_Duplicator.Invoke(source, m_History);
            if (!m_History.ContainsKey(refHash))
            {
                m_History.Add(refHash, result);
            }
            return result;
        }
    }
}
