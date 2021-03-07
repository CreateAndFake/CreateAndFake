using System;
using System.Collections.Generic;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Handles comparing specific types for the valuer.</summary>
    /// <typeparam name="T">Type to handle.</typeparam>
    public abstract class CompareHint<T> : CompareHint
    {
        /// <inheritdoc/>
        protected sealed override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return (expected is T && actual is T);
        }

        /// <inheritdoc/>
        protected sealed override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
        {
            return Compare((T)expected, (T)actual, valuer);
        }

        /// <inheritdoc cref="Compare(object,object,ValuerChainer)"/>
        protected abstract IEnumerable<Difference> Compare(T expected, T actual, ValuerChainer valuer);

        /// <inheritdoc/>
        protected sealed override int GetHashCode(object item, ValuerChainer valuer)
        {
            return GetHashCode((T)item, valuer);
        }

        /// <inheritdoc cref="GetHashCode(object,ValuerChainer)"/>
        protected abstract int GetHashCode(T item, ValuerChainer valuer);
    }
}
