namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public class InjectSample(DataSample data, DataSample data2)
{
    /// <summary>For testing.</summary>
    public DataSample Data { get; } = data;

    /// <summary>For testing.</summary>
    public DataSample Data2 { get; } = data2;
}
