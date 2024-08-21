namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples;

/// <summary>For testing.</summary>
public sealed class MethodThrowsSample
{
    /// <summary>For testing.</summary>
    public void ThrowSomething()
    {
        throw new InvalidOperationException(GetType().Name);
    }
}
