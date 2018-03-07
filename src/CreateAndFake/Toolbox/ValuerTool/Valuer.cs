using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Compares objects by value.</summary>
    public sealed class Valuer : IValuer, IDuplicatable
    {
        /// <summary>Default set of hints to use for comparisons.</summary>
        private static readonly CompareHint[] s_DefaultHints = new CompareHint[]
        {
            new EarlyFailCompareHint(),
            new FakedCompareHint(),
            new ValueEquatableCompareHint(),
            new ValuerEquatableCompareHint(),
            new EquatableCompareHint(),
            new DictionaryCompareHint(),
            new StringDictionaryCompareHint(),
            new EnumerableCompareHint(),
            new ObjectCompareHint(BindingFlags.Public | BindingFlags.Instance),
            new ObjectCompareHint(BindingFlags.NonPublic | BindingFlags.Instance),
            new StatelessCompareHint()
        };

        /// <summary>Hints used to compare specific types.</summary>
        private readonly IEnumerable<CompareHint> m_Hints;

        /// <summary>Sets up the valuer capabilities.</summary>
        /// <param name="includeDefaultHints">If the default set of hints should be added.</param>
        /// <param name="hints">Hints used to compare specific types.</param>
        public Valuer(bool includeDefaultHints = true, params CompareHint[] hints)
        {
            var inputHints = hints ?? Enumerable.Empty<CompareHint>();
            if (includeDefaultHints)
            {
                m_Hints = inputHints.Concat(s_DefaultHints).ToArray();
            }
            else
            {
                m_Hints = inputHints.ToArray();
            }
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
        [SuppressMessage("Microsoft.Design",
            "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "Necessary and expected.")]
        public int GetHashCode(object item)
        {
            (bool, int) result = m_Hints
                .Select(h => h.TryGetHashCode(item, this))
                .FirstOrDefault(r => r.Item1);

            if (!result.Equals(default))
            {
                return result.Item2;
            }
            else
            {
                throw new NotSupportedException(
                    "Type '" + item?.GetType().FullName + "' not supported by the valuer. " +
                    "Create a hint to generate the type and pass it to the valuer.");
            }
        }

        /// <summary>Returns a hash code for the specified objects.</summary>
        /// <param name="items">Objects to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        /// <exception cref="NotSupportedException">If no hint supports hashing an object.</exception>
        public int GetHashCode(params object[] items)
        {
            return GetHashCode((object)items);
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <returns>Found differences.</returns>
        /// <exception cref="NotSupportedException">If no hint supports comparing the objects.</exception>
        public IEnumerable<Difference> Compare(object expected, object actual)
        {
            if (ReferenceEquals(expected, actual))
            {
                return Enumerable.Empty<Difference>();
            }

            (bool, IEnumerable<Difference>) result = m_Hints
                .Select(h => h.TryCompare(expected, actual, this))
                .FirstOrDefault(r => r.Item1);

            if (!result.Equals(default))
            {
                return result.Item2;
            }
            else
            {
                throw new NotSupportedException(
                    "Type '" + expected?.GetType().FullName + "' not supported by the valuer. " +
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
            return new Valuer(false, duplicator.Copy(m_Hints).ToArray());
        }
    }
}
