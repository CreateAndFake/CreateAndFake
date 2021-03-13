using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;

namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <inheritdoc cref="IValuer"/>
    public sealed class Valuer : IValuer, IDuplicatable
    {
        /// <summary>Default set of hints to use for comparisons.</summary>
        private static readonly CompareHint[] _DefaultHints = new CompareHint[]
        {
            new EarlyFailCompareHint(),
            new FakedCompareHint(),
            new TaskCompareHint(),
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
        private readonly IList<CompareHint> _hints;

        /// <summary>Initializes a new instance of the <see cref="Valuer"/> class.</summary>
        /// <param name="includeDefaultHints">If the default set of hints should be added.</param>
        /// <param name="hints">Hints used to compare specific types.</param>
        public Valuer(bool includeDefaultHints = true, params CompareHint[] hints)
        {
            IEnumerable<CompareHint> inputHints = hints ?? Enumerable.Empty<CompareHint>();
            _hints = (includeDefaultHints)
                ? inputHints.Concat(_DefaultHints).ToList()
                : inputHints.ToList();
        }

        /// <inheritdoc/>
        public new bool Equals(object x, object y)
        {
            return !Compare(x, y).Any();
        }

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Design",
            "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Forwarded.")]
        public int GetHashCode(object item)
        {
            string typeName = item?.GetType().Name;
            try
            {
                return GetHashCode(item, new ValuerChainer(this, GetHashCode, Compare));
            }
            catch (InsufficientExecutionStackException)
            {
                throw new InsufficientExecutionStackException(
                    $"Ran into infinite generation trying to hash type '{typeName}'.");
            }
        }

        /// <param name="item">Object to generate a code for.</param>
        /// <param name="chainer">Handles callback behavior for child values.</param>
        /// <inheritdoc cref="GetHashCode(object)"/>
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

        /// <inheritdoc/>
        public int GetHashCode(params object[] items)
        {
            return GetHashCode((object)items);
        }

        /// <inheritdoc/>
        public IEnumerable<Difference> Compare(object expected, object actual)
        {
            string typeName = (expected ?? actual)?.GetType().Name;
            try
            {
                return Compare(expected, actual, new ValuerChainer(this, GetHashCode, Compare));
            }
            catch (InsufficientExecutionStackException)
            {
                throw new InsufficientExecutionStackException(
                    $"Ran into infinite generation trying to compare type '{typeName}'.");
            }
        }

        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="chainer">Handles callback behavior for child values.</param>
        /// <inheritdoc cref="Compare(object,object)"/>
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

        /// <inheritdoc/>
        public IDuplicatable DeepClone(IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return new Valuer(false, duplicator.Copy(_hints).ToArray());
        }

        /// <inheritdoc/>
        public void AddHint(CompareHint hint)
        {
            _hints.Insert(0, hint ?? throw new ArgumentNullException(nameof(hint)));
        }
    }
}
