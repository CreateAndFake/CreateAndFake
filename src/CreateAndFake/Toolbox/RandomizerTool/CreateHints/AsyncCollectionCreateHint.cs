using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of async collections for the randomizer.</summary>
    public sealed class AsyncCollectionCreateHint : CreateCollectionHint
    {
        /// <summary>Size details for created collections.</summary>
        private readonly int _minSize, _range;

        /// <summary>Handles creating internal representation.</summary>
        private readonly CollectionCreateHint _listHint;

        /// <summary>Initializes a new instance of the <see cref="AsyncCollectionCreateHint"/> class.</summary>
        /// <param name="minSize">Min size for created collections.</param>
        /// <param name="range">Size variance for created collections.</param>
        /// <remarks>Specifies the size of generated collections.</remarks>
        public AsyncCollectionCreateHint(int minSize = 1, int range = 3)
        {
            _minSize = minSize;
            _range = range;
            _listHint = new CollectionCreateHint(minSize, range);
        }

        /// <inheritdoc/>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            return TryCreate(type, () => _listHint.TryCreate(
                typeof(List<>).MakeGenericType(type.GetGenericArguments().Single()), randomizer).Item2);
        }

        /// <inheritdoc/>
        protected internal override (bool, object) TryCreate(Type type, int size, RandomizerChainer randomizer)
        {
            return TryCreate(type, () => _listHint.TryCreate(
                typeof(List<>).MakeGenericType(type.GetGenericArguments().Single()), size, randomizer).Item2);
        }

        /// <inheritdoc cref="TryCreate(Type, RandomizerChainer)"/>
        /// <param name="type">Type to generate.</param>
        /// <param name="listMaker">Creates the backing data.</param>
        private (bool, object) TryCreate(Type type, Func<object> listMaker)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type.Inherits(typeof(IAsyncEnumerable<>)))
            {
                return (true, GetType()
                    .GetMethod(nameof(GetItems), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(type.GetGenericArguments().Single())
                    .Invoke(null, new object[] { listMaker() }));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Generates items asynchronously.</summary>
        /// <typeparam name="T">Item type to generate.</typeparam>
        /// <param name="backing">Items to generate.</param>
        /// <returns>The generated items.</returns>
        private static async IAsyncEnumerable<T> GetItems<T>(List<T> backing)
        {
            for (int i = 0; i < backing.Count; i++)
            {
                await Task.Yield();
                yield return backing[i];
            }
        }
    }
}