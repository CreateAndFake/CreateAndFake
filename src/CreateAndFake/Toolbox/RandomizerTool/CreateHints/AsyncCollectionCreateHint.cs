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

        /// <summary>Initializes a new instance of the <see cref="AsyncCollectionCreateHint"/> class.</summary>
        /// <param name="minSize">Min size for created collections.</param>
        /// <param name="range">Size variance for created collections.</param>
        /// <remarks>Specifies the size of generated collections.</remarks>
        public AsyncCollectionCreateHint(int minSize = 1, int range = 3)
        {
            _minSize = minSize;
            _range = range;
        }

        /// <inheritdoc/>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            return TryCreate(type, _minSize + randomizer?.Gen.Next(_range) ?? 0, randomizer);
        }

        /// <inheritdoc/>
        protected internal override (bool, object) TryCreate(Type type, int size, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            if (type.AsGenericType() == typeof(IAsyncEnumerator<>))
            {
                return (true, GetType()
                    .GetMethod(nameof(GetItems), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(type.GetGenericArguments().Single())
                    .Invoke(null, new object[] { size, randomizer }));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Generates items asynchronously.</summary>
        /// <typeparam name="T">Item type to generate.</typeparam>
        /// <param name="count">Number of items to generate.</param>
        /// <param name="randomizer">Randomizer for the actual items.</param>
        /// <returns>The generated items.</returns>
        private static async IAsyncEnumerable<T> GetItems<T>(int count, RandomizerChainer randomizer)
        {
            for (int i = 0; i < count; i++)
            {
                await Task.Delay(0).ConfigureAwait(false);
                yield return randomizer.Create<T>();
            }
        }
    }
}