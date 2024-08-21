namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
public class FieldSample(string stringValue, string stringValue2)
{
    /// <summary>For testing.</summary>
    public const int Perm = 1;

    /// <summary>For testing.</summary>
    public readonly string PermText = stringValue2;

    /// <summary>For testing.</summary>
    public string StringValue = stringValue;

    /// <summary>For testing.</summary>
    public int NumberValue;

    /// <summary>For testing.</summary>
    public IEnumerable<string> CollectionValue;
}
