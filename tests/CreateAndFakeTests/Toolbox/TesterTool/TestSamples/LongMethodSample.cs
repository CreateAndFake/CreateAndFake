using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples
{
    /// <summary>For testing.</summary>
    public static class LongMethodSample
    {
        /// <summary>For testing.</summary>
        [SuppressMessage("IDE", "IDE0059:UnnecessaryAssignment", Justification = "False.")]
        public static void BeSlow<T>(string data, out T output) where T : new()
        {
            Thread.Sleep(new TimeSpan(0, 0, 2));

            output = new T();
            throw new InvalidOperationException(data);
        }
    }
}
