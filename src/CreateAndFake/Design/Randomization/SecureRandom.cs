using System.Security.Cryptography;

namespace CreateAndFake.Design.Randomization
{
    /// <summary>For generating secure but slow random values.</summary>
    public sealed class SecureRandom : ValueRandom
    {
        /// <inheritdoc/>
        public override int? InitialSeed { get; } = null;

        /// <summary>Source generator used for random bytes.</summary>
        private static readonly RandomNumberGenerator _Gen = RandomNumberGenerator.Create();

        /// <summary>Sets up the randomizer.</summary>
        /// <param name="onlyValidValues">Option to prevent generating invalid values.</param>
        public SecureRandom(bool onlyValidValues = true) : base(onlyValidValues) { }

        /// <inheritdoc/>
        protected override byte[] NextBytes(short length)
        {
            byte[] buffer = new byte[length];
            _Gen.GetBytes(buffer);
            return buffer;
        }
    }
}
