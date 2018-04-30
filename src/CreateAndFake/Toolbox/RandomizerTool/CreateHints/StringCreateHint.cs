using System;
using System.Collections.Generic;
using System.Linq;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of strings for the randomizer.</summary>
    public sealed class StringCreateHint : CreateHint<string>
    {
        /// <summary>Default characters to include in random strings.</summary>
        private static readonly char[] s_DefaultCharSet =
            @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        /// <summary>Characters to include in random strings.</summary>
        private readonly char[] m_CharSet;

        /// <summary>Size details for created strings.</summary>
        private readonly int m_MinSize, m_Range;

        /// <summary>Specifies the given character set for string randomization.</summary>
        /// <param name="minSize">Min size for created collections.</param>
        /// <param name="range">Size variance for created collections.</param>
        /// <param name="charSet">Character set to use for randomization.</param>
        public StringCreateHint(int minSize = 7, int range = 5, IEnumerable<char> charSet = null)
        {
            m_MinSize = minSize;
            m_Range = range;

            if (charSet != null && charSet.Any())
            {
                m_CharSet = charSet.ToArray();
            }
            else
            {
                m_CharSet = s_DefaultCharSet;
            }
        }

        /// <summary>Creates a random instance of the type.</summary>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>Created instance.</returns>
        protected override string Create(RandomizerChainer randomizer)
        {
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            char[] data = new char[m_MinSize + randomizer.Gen.Next(m_Range)];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = randomizer.Gen.NextItem(m_CharSet);
            }
            return new string(data);
        }
    }
}
