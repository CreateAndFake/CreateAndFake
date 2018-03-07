namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public abstract class ConstraintSample<TStruct, TClass>
        where TStruct : struct
        where TClass : DataSample, new()
    {
        /// <summary>For testing.</summary>
        public TStruct StructValue { get; set; }

        /// <summary>For testing.</summary>
        public TClass ClassValue { get; set; }
    }
}
