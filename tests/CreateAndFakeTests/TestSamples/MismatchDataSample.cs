namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public class MismatchDataSample(int value)
{
    /// <summary>For testing.</summary>
    public string Data { get; set; } = "Value:" + value;
}
