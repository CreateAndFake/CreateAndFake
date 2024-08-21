using System.Diagnostics.CodeAnalysis;

namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
[ExcludeFromCodeCoverage]
public sealed class InvalidCreateSample : IOnlyMockSample
{
    /// <summary>For testing.</summary>
    public InvalidCreateSample()
    {
        throw new InvalidOperationException("Tried to create invalid sample.");
    }

    /// <summary>For testing.</summary>
    public bool FailIfNotMocked()
    {
        throw new InvalidOperationException("Mock was not created.");
    }
}
