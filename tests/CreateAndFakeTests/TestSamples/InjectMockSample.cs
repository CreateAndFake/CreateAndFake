using System;
using CreateAndFake;

namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
/// <remarks>For testing.</remarks>
public sealed class InjectMockSample(IOnlyMockSample sample1, IOnlyMockSample sample2)
{
    /// <summary>For testing.</summary>
    private readonly IOnlyMockSample
        _sample1 = sample1 ?? throw new ArgumentNullException(nameof(sample1)),
        _sample2 = sample2 ?? throw new ArgumentNullException(nameof(sample2));

    /// <summary>For testing.</summary>
    public void TestIfMockedSeparately()
    {
        _sample1.FailIfNotMocked();
        _sample2.FailIfNotMocked();
        Tools.Asserter.IsNot(_sample1, _sample2);
    }
}
