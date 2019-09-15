using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Compares objects by value.</summary>
    public sealed class Valuer : IValuer, IDuplicatable
    {
        /// <summary>Default set of hints to use for comparisons.</summary>
        private static readonly CompareHint[] _DefaultHints = new CompareHint[]
        {
            new EarlyFailCompareHint(),
            new FakedCompareHint(),
            new ValueEquatableCompareHint(),
            new ValuerEquatableCompareHint(),
            new EquatableCompareHint(),
            new StringDictionaryCompareHint(),
            new DictionaryCompareHint(),
            new EnumerableCompareHint(),
            new ObjectCompareHint(BindingFlags.Public | BindingFlags.Instance),
            new ObjectCompareHint(BindingFlags.NonPublic | BindingFlags.Instance),
            new StatelessCompareHint()
        };

        /// <summary>Hints used to compare specific types.</summary>
        private readonly IEnumerable<CompareHint> _hints;

        /// <summary>Sets up the valuer capabilities.</summary>
        /// <param name="includeDefaultHints">If the default set of hints should be added.</param>
        /// <param name="hints">Hints used to compare specific types.</param>
        public Valuer(bool includeDefaultHints = true, params CompareHint[] hints)
        {
            IEnumerable<CompareHint> inputHints = hints ?? Enumerable.Empty<CompareHint>();
            if (includeDefaultHints)
            {
                _hints = inputHints.Concat(_DefaultHints).ToArray();
            }
            else
            {
                _hints = inputHints.ToArray();
            }
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if equal; false otherwise.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        public new bool Equals(object x, object y)
        {
            return !Compare(x, y).Any();
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing the object.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Forwarded.")]
        public int GetHashCode(object item)
        {
            try
            {
                return GetHashCode(item, new ValuerChainer(this, GetHashCode, Compare));
            }
            catch (InsufficientExecutionStackException)
            {
                throw new InsufficientExecutionStackException(
                    $"Ran into infinite generation trying to hash type '{item?.GetType().Name}'.");
            }
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="chainer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing the object.</exception>
        private int GetHashCode(object item, ValuerChainer chainer)
        {
            (bool, int) result = _hints
                .Select(h => h.TryGetHashCode(item, chainer))
                .FirstOrDefault(r => r.Item1);

            if (!result.Equals(default))
            {
                return result.Item2;
            }
            else
            {
                throw new NotSupportedException(
                    $"Type '{item?.GetType().FullName}' not supported by the valuer. " +
                    "Create a hint to generate the type and pass it to the valuer.");
            }
        }

        /// <summary>Returns a hash code for the specified objects.</summary>
        /// <param name="items">Objects to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing an object.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        public int GetHashCode(params object[] items)
        {
            return GetHashCode((object)items);
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <returns>Found differences.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        public IEnumerable<Difference> Compare(object expected, object actual)
        {
            try
            {
                return Compare(expected, actual, new ValuerChainer(this, GetHashCode, Compare));
            }
            catch (InsufficientExecutionStackException)
            {
                throw new InsufficientExecutionStackException(
                    $"Ran into infinite generation trying to compare type '{expected?.GetType().Name}'.");
            }
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="chainer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        private IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer chainer)
        {
            if (ReferenceEquals(expected, actual))
            {
                return Enumerable.Empty<Difference>();
            }

            (bool, IEnumerable<Difference>) result = _hints
                .Select(h => h.TryCompare(expected, actual, chainer))
                .FirstOrDefault(r => r.Item1);

            if (!result.Equals(default))
            {
                return result.Item2;
            }
            else
            {
                throw new NotSupportedException(
                    $"Type '{expected?.GetType().FullName}' not supported by the valuer. " +
                    "Create a hint to generate the type and pass it to the valuer.");
            }
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <param name="duplicator">Duplicator to clone child values.</param>
        /// <returns>Clone that is equal in value to the instance.</returns>
        public IDuplicatable DeepClone(IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return new Valuer(false, duplicator.Copy(_hints).ToArray());
        }
    }
}
