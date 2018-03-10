using System;
using System.Runtime.CompilerServices;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.DuplicatorTool
{
    /// <summary>Provides a callback into the duplicator to create child values.</summary>
    public sealed class DuplicatorChainer
    {
        /// <summary>Callback to the duplicator to handle child values.</summary>
        private readonly Func<object, DuplicatorChainer, object> m_Callback;

        /// <summary>History of clones to match up references.</summary>
        private readonly ConditionalWeakTable<object, object> m_History;

        /// <summary>Reference to the actual duplicator.</summary>
        internal IDuplicator Duplicator { get; }

        /// <summary>Sets up the callback functionality.</summary>
        /// <param name="duplicator">Reference to the actual duplicator.</param>
        /// <param name="callback">Callback to the duplicator to handle child values.</param>
        public DuplicatorChainer(IDuplicator duplicator, Func<object, DuplicatorChainer, object> callback)
        {
            Duplicator = duplicator ?? throw new ArgumentNullException(nameof(duplicator));
            m_Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            m_History = new ConditionalWeakTable<object, object>();
        }

        /// <summary>Adds created type to history.</summary>
        /// <param name="source">Object cloned.</param>
        /// <param name="clone">The clone.</param>
        public void AddToHistory(object source, object clone)
        {
            if (CanTrack(source))
            {
                m_History.Add(source, clone);
            }
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
            if (!CanTrack(source))
            {
                return m_Callback.Invoke(source, this);
            }

            if (m_History.TryGetValue(source, out object clone))
            {
                return clone;
            }

            object result = m_Callback.Invoke(source, this);
            if (!m_History.TryGetValue(source, out object blank))
            {
                m_History.Add(source, result);
            }
            return result;
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
