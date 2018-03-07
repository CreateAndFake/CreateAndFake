namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class MismatchDataSample
    {
        /// <summary>For testing.</summary>
        public string Data { get; set; }

        /// <summary>For testing.</summary>
        public MismatchDataSample(int value)
        {
            Data = "Value:" + value;
        }
    }
}
