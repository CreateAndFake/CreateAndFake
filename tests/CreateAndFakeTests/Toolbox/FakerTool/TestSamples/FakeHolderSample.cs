namespace CreateAndFakeTests.Toolbox.FakerTool.TestSamples
{
    /// <summary>For testing.</summary>
    public sealed class FakeHolderSample
    {
        /// <summary>For testing.</summary>
        public IFakeSample Sample1 { get; }

        /// <summary>For testing.</summary>
        public AbstractFakeSample Sample2 { get; }

        /// <summary>For testing.</summary>
        public int Value1 { get; }

        /// <summary>For testing.</summary>
        public string Value2 { get; }

        /// <summary>For testing.</summary>
        public FakeHolderSample(IFakeSample sample1, AbstractFakeSample sample2, int value1, string value2)
        {
            Sample1 = sample1;
            Sample2 = sample2;
            Value1 = value1;
            Value2 = value2;
        }
    }
}
