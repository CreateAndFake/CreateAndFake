using System;
using System.Collections.Generic;
using System.Linq;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of strings for the randomizer.</summary>
    public sealed class StringCreateHint : CreateHint<string>
    {
        /// <summary>Default characters to include in random strings.</summary>
        private static readonly char[] _DefaultCharSet =
            @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        /// <summary>Characters to include in random strings.</summary>
        private readonly char[] _charSet;

        /// <summary>Size details for created strings.</summary>
        private readonly int _minSize, _range;

        /// <summary>Initializes a new instance of the <see cref="StringCreateHint"/> class.</summary>
        /// <param name="minSize">Min size for created collections.</param>
        /// <param name="range">Size variance for created collections.</param>
        /// <param name="charSet">Character set to use for randomization.</param>
        public StringCreateHint(int minSize = 7, int range = 5, IEnumerable<char> charSet = null)
        {
            _minSize = minSize;
            _range = range;
            _charSet = (charSet?.Any() ?? false)
                ? charSet.ToArray()
                : _DefaultCharSet;
        }

        /// <inheritdoc/>
        protected override string Create(RandomizerChainer randomizer)
        {
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            char[] data = new char[_minSize + randomizer.Gen.Next(_range)];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = randomizer.Gen.NextItem(_charSet);
            }
            return new string(data);
        }
    }
}
