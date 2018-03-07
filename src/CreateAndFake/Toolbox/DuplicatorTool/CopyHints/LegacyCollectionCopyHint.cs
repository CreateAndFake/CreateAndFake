using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying legacy collections for the duplicator.</summary>
    public sealed class LegacyCollectionCopyHint : CopyHint
    {
        /// <summary>Supported types and the methods used to generate them.</summary>
        private static readonly IDictionary<Type, Func<object, IDuplicator, object>> s_Copiers
            = new Dictionary<Type, Func<object, IDuplicator, object>>
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
                        StringCollection result = new StringCollection();
                        foreach (string item in (StringCollection)data)
                        {
                            result.Add(item);
                        }
                        return result;
                    }
                },
                { typeof(StringDictionary), (data, cloner) =>
                    {
                        StringDictionary result = new StringDictionary();
                        foreach (DictionaryEntry entry in (StringDictionary)data)
                        {
                            result.Add((string)entry.Key, (string)entry.Value);
                        }
                        return result;
                    }
                }
            };

        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Duplicator to handle child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal override sealed (bool, object) TryCopy(object source, IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return (true, null);

            if (s_Copiers.TryGetValue(source.GetType(), out var copier))
            {
                return (true, copier.Invoke(source, duplicator));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Clones a collection.</summary>
        /// <typeparam name="T">Collection type being cloned.</typeparam>
        /// <param name="source">Collection to clone.</param>
        /// <param name="duplicator">Duplicator to handle child values.</param>
        /// <returns>The cloned instance.</returns>
        private static T CreateAndCopy<T>(object source, IDuplicator duplicator) where T : IDictionary, new()
        {
            T result = new T();
            foreach (DictionaryEntry entry in (T)source)
            {
                result.Add(duplicator.Copy(entry.Key), duplicator.Copy(entry.Value));
            }
            return result;
        }
    }
}
