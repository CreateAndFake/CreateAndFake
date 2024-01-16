namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public abstract class GenericSample<TClass>
{
    /// <summary>For testing.</summary>
    public abstract string Run<TMethod>(TMethod input);

    /// <summary>For testing.</summary>
    public abstract TReturn Run<TMethod, TReturn>(TClass in1, TMethod in2)
        where TMethod : DataSample
        where TReturn : new();
}
