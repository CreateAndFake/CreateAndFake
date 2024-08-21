﻿using System.Collections;
using System.Collections.Specialized;
using CreateAndFake.Design;

#pragma warning disable IDE0058 // Expression value is never used: Return not available on all versions.

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Handles cloning legacy collections for <see cref="IDuplicator"/> .</summary>
public sealed class LegacyCollectionCopyHint : CopyHint
{
    /// <summary>Supported types and the methods used to generate them.</summary>
    private static readonly Dictionary<Type, Func<object, DuplicatorChainer, object>> _Copiers = new()
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
                    StringCollection result = [.. (StringCollection)data];
                    return result;
                }
            },
            { typeof(StringDictionary), (data, cloner) =>
                {
                    StringDictionary result = [];
                    foreach (DictionaryEntry entry in (StringDictionary)data)
                    {
                        result.Add((string)entry.Key, (string?)entry.Value);
                    }
                    return result;
                }
            }
        };

    /// <inheritdoc/>
    protected internal sealed override (bool, object?) TryCopy(object source, DuplicatorChainer duplicator)
    {
        ArgumentGuard.ThrowIfNull(source, nameof(source));
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        if (_Copiers.TryGetValue(source.GetType(), out Func<object, DuplicatorChainer, object>? copier))
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
            result.Add(duplicator.Copy(entry.Key)!, duplicator.Copy(entry.Value));
        }
        return result;
    }
}

#pragma warning restore IDE0058 // Expression value is never used
