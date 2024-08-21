using System.Diagnostics.CodeAnalysis;

namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public class IsBadSample : IIsGoodOrBadSample
{
    /// <summary>For testing.</summary>
    [ExcludeFromCodeCoverage]
    public int GoodOrBadProp
    {
        get => 0;
        set => throw new NotImplementedException();
    }
}
