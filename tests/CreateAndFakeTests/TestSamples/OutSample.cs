namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public abstract class OutSample
{
    /// <summary>For testing.</summary>
    /// <param name="input">For testing.</param>
    public abstract void ReturnVoid(out string input);

    /// <summary>For testing.</summary>
    /// <param name="input">For testing.</param>
    public abstract int ReturnValue(out int input);
}
