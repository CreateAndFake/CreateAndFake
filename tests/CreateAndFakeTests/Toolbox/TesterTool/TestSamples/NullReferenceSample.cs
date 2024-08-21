using System.Diagnostics.CodeAnalysis;
using CreateAndFake.Design.Content;

namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples;

/// <summary>For testing.</summary>
public sealed class NullReferenceSample
{
    /// <summary>For testing.</summary>
    private readonly IValueEquatable _data;

    /// <summary>For testing.</summary>
    internal NullReferenceSample(IValueEquatable data)
    {
        _data = data;
    }

    /// <summary>For testing.</summary>
    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return _data.ToString();
    }
}
