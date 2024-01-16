namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public sealed class FactorySample
{
    /// <summary>For testing.</summary>
    public string Data { get; }

    /// <summary>For testing.</summary>
    /// <param name="data">For testing.</param>
    private FactorySample(string data)
    {
        Data = data;
    }

    /// <summary>For testing.</summary>
    /// <param name="data">For testing.</param>
    /// <returns>For testing.</returns>
    internal static FactorySample Create(int data)
    {
        return new FactorySample("Value:" + data);
    }
}
