using System.Diagnostics.CodeAnalysis;

namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public interface IUnimplementedSample
{
    /// <summary>For testing.</summary>
    int Flag { get; }

    /// <summary>For testing.</summary>
    [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "For testing.")]
    bool Funny { set; }

    /// <summary>For testing.</summary>
    /// <returns>Dummy value.</returns>
    string GetData();
}
