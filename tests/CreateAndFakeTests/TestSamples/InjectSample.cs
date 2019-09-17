namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class InjectSample
    {
        /// <summary>For testing.</summary>
        public DataSample Data { get; }

        /// <summary>For testing.</summary>
        public DataSample Data2 { get; }

        /// <summary>For testing.</summary>
        public InjectSample(DataSample data, DataSample data2)
        {
            Data = data;
            Data2 = data2;
        }
    }
}
