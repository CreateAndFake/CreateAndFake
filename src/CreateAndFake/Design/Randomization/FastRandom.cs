﻿using System;

#pragma warning disable CA5394 // Do not use insecure randomness

namespace CreateAndFake.Design.Randomization
{
    /// <summary>For generating quick but insecure random values.</summary>
    public sealed class FastRandom : ValueRandom
    {
        /// <summary>Source generator used for random bytes.</summary>
        private static readonly Random _Gen = new();

        /// <summary>Sets up the randomizer.</summary>
        /// <param name="onlyValidValues">Option to prevent generating invalid values.</param>
        public FastRandom(bool onlyValidValues = true) : base(onlyValidValues) { }

        /// <summary>Generates a byte array filled with random bytes.</summary>
        /// <param name="length">Length of the array to generate.</param>
        /// <returns>The generated byte array.</returns>
        protected override byte[] NextBytes(short length)
        {
            byte[] buffer = new byte[length];
            lock (_Gen)
            {
                _Gen.NextBytes(buffer);
            }
            return buffer;
        }
    }
}

#pragma warning restore CA5394 // Do not use insecure randomness
