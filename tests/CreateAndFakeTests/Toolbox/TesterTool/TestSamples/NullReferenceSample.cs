using CreateAndFake.Design.Content;

namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples
{
    /// <summary>For testing.</summary>
    public sealed class NullReferenceSample
    {
        /// <summary>For testing.</summary>
        private readonly IValueEquatable m_Data;

        /// <summary>For testing.</summary>
        public NullReferenceSample(IValueEquatable data)
        {
            m_Data = data;
        }

        /// <summary>For testing.</summary>
        public override string ToString()
        {
            return m_Data.ToString();
        }
    }
}
