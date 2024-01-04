namespace CreateAndFakeTests.Toolbox.FakerTool.TestSamples;

/// <summary>For testing.</summary>
public sealed class FakeHolderSample(IFakeSample sample1, AbstractFakeSample sample2, int value1, string value2)
{
    /// <summary>For testing.</summary>
    public IFakeSample Sample1 { get; } = sample1;

    /// <summary>For testing.</summary>
    public AbstractFakeSample Sample2 { get; } = sample2;

    /// <summary>For testing.</summary>
    public int Value1 { get; } = value1;

    /// <summary>For testing.</summary>
    public string Value2 { get; } = value2;
}
