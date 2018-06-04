namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class InjectSample
    {
        /// <summary>For testing.</summary>
        public DataSample Data { get; }

        /// <summary>For testing.</summary>
        public InjectSample(DataSample data)
        {
            Data = data;
        }
    }
}
