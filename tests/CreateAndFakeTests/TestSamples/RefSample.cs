namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public abstract class RefSample
    {
        /// <summary>For testing.</summary>
        /// <param name="input">For testing.</param>
        public abstract void ReturnVoid(ref string input);

        /// <summary>For testing.</summary>
        /// <param name="input">For testing.</param>
        public abstract int ReturnValue(ref int input);
    }
}
