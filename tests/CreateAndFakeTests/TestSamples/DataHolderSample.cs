namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class DataHolderSample : DataSample
    {
        /// <summary>For testing.</summary>
        public DataSample NestedValue { get; set; }

        /// <summary>For testing.</summary>
        /// <param name="value">Test value.</param>
        /// <returns>Test value.</returns>
        public virtual bool HasNested(DataSample value)
        {
            return false;
        }
    }
}
