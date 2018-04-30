using System;

namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples
{
    /// <summary>For testing.</summary>
    public sealed class MismatchParamNameSample
    {
        /// <summary>For testing.</summary>
        public MismatchParamNameSample(object data, object data2)
        {
            if (data == null) throw new ArgumentNullException(nameof(data2));
            if (data2 == null) throw new ArgumentNullException(nameof(data));
        }
    }
}
