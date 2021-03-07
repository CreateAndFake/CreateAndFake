using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing dictionaries for the valuer.</summary>
    public sealed class DictionaryCompareHint : CompareHint<IDictionary>
    {
        /// <inheritdoc/>
        protected override IEnumerable<Difference> Compare(
            IDictionary expected, IDictionary actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return LazyCompare(expected, actual, valuer);
        }

        /// <inheritdoc cref="Compare"/>
        private static IEnumerable<Difference> LazyCompare(
            IDictionary expected, IDictionary actual, ValuerChainer valuer)
        {
            object[] expectedKeys = expected.Keys.Cast<object>().ToArray();
            object[] actualKeys = actual.Keys.Cast<object>().ToArray();

            foreach (object key in expectedKeys)
            {
                object match = actualKeys.FirstOrDefault(k => valuer.Equals(key, k));
                if (match != null)
                {
                    foreach (Difference diff in valuer.Compare(expected[key], actual[match]))
                    {
                        yield return new Difference($"[{key}]", diff);
                    }
                }
                else
                {
                    yield return new Difference($"[{key}]", new Difference(expected[key], "'null'"));
                }
            }

            foreach (object key in actualKeys)
            {
                if (!expectedKeys.Any(k => valuer.Equals(key, k)))
                {
                    yield return new Difference($"[{key}]", new Difference("'null'", actual[key]));
                }
            }
        }

        /// <inheritdoc/>
        protected override int GetHashCode(IDictionary item, ValuerChainer valuer)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            int hash = ValueComparer.BaseHash;
            foreach (DictionaryEntry entry in item)
            {
                hash += valuer.GetHashCode(entry.Key, entry.Value);
            }
            return hash;
        }
    }
}
