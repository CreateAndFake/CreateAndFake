using System;
using System.Diagnostics.CodeAnalysis;

namespace CreateAndFakeTests.Toolbox.FakerTool.TestSamples;

/// <summary>For testing.</summary>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
    Justification = "False positive; created by the activator.")]
[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
    Justification = "For testing.")]
public class VirtualFakeSample : AbstractFakeSample
{
    /// <summary>For testing.</summary>
    public override int Num => throw new NotImplementedException();

    /// <summary>For testing.</summary>
    public override string Hint => throw new NotImplementedException();

    /// <summary>For testing.</summary>
    public override int Count
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>For testing.</summary>
    public override string Text
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>For testing.</summary>
    public override int Calc()
    {
        throw new NotImplementedException();
    }

    /// <summary>For testing.</summary>
    public override int Calc(int data)
    {
        throw new NotImplementedException();
    }

    /// <summary>For testing.</summary>
    public override void Combo(int num, string text)
    {
        throw new NotImplementedException();
    }

    /// <summary>For testing.</summary>
    public override string Read()
    {
        throw new NotImplementedException();
    }

    /// <summary>For testing.</summary>
    public override string Read(string data)
    {
        throw new NotImplementedException();
    }
}
