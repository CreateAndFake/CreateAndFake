using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing string dictionaries for the valuer.</summary>
    public sealed class StringDictionaryCompareHint : CompareHint<StringDictionary>
    {
        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(
            StringDictionary expected, StringDictionary actual, ValuerChainer valuer)
        {
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return valuer.Compare(Convert(expected), Convert(actual));
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(StringDictionary item, ValuerChainer valuer)
        {
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return valuer.GetHashCode(Convert(item));
        }

        /// <summary>Handles changing string dictionaries to dictionaries.</summary>
        /// <param name="dict">Dictionary to convert.</param>
        /// <returns>Converted dictionary.</returns>
        private static IDictionary Convert(StringDictionary dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            return dict.Cast<DictionaryEntry>().ToDictionary(e => e.Key, e => e.Value);
        }
    }
}
