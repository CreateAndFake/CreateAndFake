using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing dictionaries for the valuer.</summary>
    public sealed class DictionaryCompareHint : CompareHint<IDictionary>
    {
        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(
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
                        yield return new Difference("[" + key + "]", diff);
                    }
                }
                else
                {
                    yield return new Difference("[" + key + "]", new Difference(expected[key], "'null'"));
                }
            }

            foreach (object key in actualKeys)
            {
                if (!expectedKeys.Any(k => valuer.Equals(key, k)))
                {
                    yield return new Difference("[" + key + "]", new Difference("'null'", actual[key]));
                }
            }
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(IDictionary item, ValuerChainer valuer)
        {
            int hash = ValueComparer.BaseHash;
            foreach (DictionaryEntry entry in item)
            {
                hash += valuer.GetHashCode(entry.Key, entry.Value);
            }
            return hash;
        }
    }
}
