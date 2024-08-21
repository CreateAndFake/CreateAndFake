namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public interface IUnimplementedSample
{
    /// <summary>For testing.</summary>
    int Flag { get; }

    /// <summary>For testing.</summary>
    bool Funny { set; }

    /// <summary>For testing.</summary>
    /// <returns>Dummy value.</returns>
    string GetData();
}
