using System;

namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public class IsBadSample : IIsGoodOrBadSample
{
    /// <summary>For testing.</summary>
    public int GoodOrBadProp
    {
        get => 0;
        set => throw new NotImplementedException();
    }
}
