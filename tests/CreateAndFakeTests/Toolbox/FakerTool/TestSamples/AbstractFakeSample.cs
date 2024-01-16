namespace CreateAndFakeTests.Toolbox.FakerTool.TestSamples;

/// <summary>For testing.</summary>
public abstract class AbstractFakeSample : IFakeSample
{
    /// <summary>For testing.</summary>
    public abstract int Num { get; }

    /// <summary>For testing.</summary>
    public abstract string Hint { get; }

    /// <summary>For testing.</summary>
    public abstract int Count { get; set; }

    /// <summary>For testing.</summary>
    public abstract string Text { get; set; }

    /// <summary>For testing.</summary>
    public abstract int Calc();

    /// <summary>For testing.</summary>
    public abstract int Calc(int data);

    /// <summary>For testing.</summary>
    public abstract void Combo(int num, string text);

    /// <summary>For testing.</summary>
    public abstract string Read();

    /// <summary>For testing.</summary>
    public abstract string Read(string data);
}
