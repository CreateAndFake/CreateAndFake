namespace CreateAndFakeTests.Toolbox.FakerTool.TestSamples
{
    /// <summary>For testing.</summary>
    public interface IFakeSample
    {
        /// <summary>For testing.</summary>
        int Num { get; }

        /// <summary>For testing.</summary>
        string Hint { get; }

        /// <summary>For testing.</summary>
        int Count { get; set; }

        /// <summary>For testing.</summary>
        string Text { get; set; }

        /// <summary>For testing.</summary>
        int Calc();

        /// <summary>For testing.</summary>
        int Calc(int data);

        /// <summary>For testing.</summary>
        string Read();

        /// <summary>For testing.</summary>
        string Read(string data);

        /// <summary>For testing.</summary>
        void Combo(int num, string text);
    }
}
