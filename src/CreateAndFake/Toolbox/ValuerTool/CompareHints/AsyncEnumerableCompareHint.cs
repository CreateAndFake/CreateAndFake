using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing async collections for <see cref="IValuer"/>.</summary>
    public sealed class AsyncEnumerableCompareHint : CompareHint
    {
        /// <inheritdoc/>
        protected override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));

            return expected.GetType().Inherits(typeof(IAsyncEnumerable<>))
                && actual.GetType().Inherits(typeof(IAsyncEnumerable<>));
        }

        /// <inheritdoc/>
        protected override IEnumerable<Difference> Compare(
            object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return ((Task<IEnumerable<Difference>>)GetType()
                .GetMethod(nameof(CompareAsync), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(expected.GetType().GetGenericArguments().Single())
                .Invoke(null, new object[] { expected, actual, valuer })).Result;
        }

        /// <inheritdoc cref="Compare"/>
        /// <typeparam name="T">Item type being compared.</typeparam>
        private static async Task<IEnumerable<Difference>> CompareAsync<T>(
            IAsyncEnumerable<T> expected, IAsyncEnumerable<T> actual, ValuerChainer valuer)
        {
            IAsyncEnumerator<T> expectedEnumerator = expected.GetAsyncEnumerator();
            IAsyncEnumerator<T> actualEnumerator = actual.GetAsyncEnumerator();
            int index = 0;

            List<Difference> differences = new();

            while (await expectedEnumerator.MoveNextAsync().ConfigureAwait(false))
            {
                if (await actualEnumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    differences.AddRange(valuer
                        .Compare(expectedEnumerator.Current, actualEnumerator.Current)
                        .Select(diff => new Difference(index, diff)));
                }
                else
                {
                    differences.Add(new Difference(index, new Difference(expectedEnumerator.Current, "'outofbounds'")));
                }
                index++;
            }
            while (await actualEnumerator.MoveNextAsync().ConfigureAwait(false))
            {
                differences.Add(new Difference(index++, new Difference("'outofbounds'", actualEnumerator.Current)));
            }

            return differences;
        }

        /// <inheritdoc/>
        protected override int GetHashCode(object item, ValuerChainer valuer)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return ((Task<int>)GetType()
                .GetMethod(nameof(GetHashCodeAsync), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(item.GetType().GetGenericArguments().Single())
                .Invoke(null, new object[] { item, valuer })).Result;
        }

        /// <inheritdoc cref="GetHashCode"/>
        /// <typeparam name="T">Item type being compared.</typeparam>
        private static async Task<int> GetHashCodeAsync<T>(IAsyncEnumerable<T> item, ValuerChainer valuer)
        {
            IAsyncEnumerator<T> enumerator = item.GetAsyncEnumerator();

            int hash = ValueComparer.BaseHash;
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                hash = hash * ValueComparer.HashMultiplier + valuer.GetHashCode(enumerator.Current);
            }
            return hash;
        }
    }
}
