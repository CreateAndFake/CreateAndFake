using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

// Return not available on all .NET versions.
#pragma warning disable IDE0058 // Expression value is never used

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying legacy collections for the duplicator.</summary>
    public sealed class LegacyCollectionCopyHint : CopyHint
    {
        /// <summary>Supported types and the methods used to generate them.</summary>
        private static readonly IDictionary<Type, Func<object, DuplicatorChainer, object>> _Copiers
            = new Dictionary<Type, Func<object, DuplicatorChainer, object>>
            {
                { typeof(Hashtable), CreateAndCopy<Hashtable> },
                { typeof(SortedList), CreateAndCopy<SortedList> },
                { typeof(ListDictionary), CreateAndCopy<ListDictionary> },
                { typeof(HybridDictionary), CreateAndCopy<HybridDictionary> },
                { typeof(OrderedDictionary), CreateAndCopy<OrderedDictionary> },

                { typeof(BitArray), (data, cloner) => new BitArray((BitArray)data) },
                { typeof(NameValueCollection), (data, cloner) => new NameValueCollection((NameValueCollection)data) },

                { typeof(StringCollection), (data, cloner) =>
                    {
                        StringCollection result = new();
                        foreach (string item in (StringCollection)data)
                        {
                            result.Add(item);
                        }
                        return result;
                    }
                },
                { typeof(StringDictionary), (data, cloner) =>
                    {
                        StringDictionary result = new();
                        foreach (DictionaryEntry entry in (StringDictionary)data)
                        {
                            result.Add((string)entry.Key, (string)entry.Value);
                        }
                        return result;
                    }
                }
            };

        /// <inheritdoc/>
        protected internal sealed override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return (true, null);

            if (_Copiers.TryGetValue(source.GetType(), out Func<object, DuplicatorChainer, object> copier))
            {
                return (true, copier.Invoke(source, duplicator));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Clones <paramref name="source"/>.</summary>
        /// <typeparam name="T">Collection type being cloned.</typeparam>
        /// <param name="source">Collection to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Clone of <paramref name="source"/>.</returns>
        private static T CreateAndCopy<T>(object source, DuplicatorChainer duplicator) where T : IDictionary, new()
        {
            T result = new();
            foreach (DictionaryEntry entry in (T)source)
            {
                result.Add(duplicator.Copy(entry.Key), duplicator.Copy(entry.Value));
            }
            return result;
        }
    }
}

#pragma warning restore IDE0058 // Expression value is never used
