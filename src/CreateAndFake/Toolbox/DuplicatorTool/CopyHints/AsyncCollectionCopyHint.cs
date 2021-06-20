using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying async collections for the duplicator.</summary>
    public sealed class AsyncCollectionCopyHint : CopyHint
    {
        /// <inheritdoc/>
        protected internal override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return (true, null);

            Type sourceType = source.GetType();

            if (sourceType.Inherits(typeof(IAsyncEnumerable<>)))
            {
                return (true, GetType()
                    .GetMethod(nameof(CopyAsync), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(sourceType.GetGenericArguments().Single())
                    .Invoke(null, new object[] { source, duplicator }));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Deep clones <paramref name="source"/>.</summary>
        /// <typeparam name="T">Item type being copied.</typeparam>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Clone of <paramref name="source"/>.</returns>
        private static async IAsyncEnumerable<T> CopyAsync<T>(IAsyncEnumerable<T> source, DuplicatorChainer duplicator)
        {
            await using IAsyncEnumerator<T> gen = source.GetAsyncEnumerator();

            while (await gen.MoveNextAsync().ConfigureAwait(false))
            {
                yield return duplicator.Copy(gen.Current);
            }
        }
    }
}