using System.Diagnostics.CodeAnalysis;

namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Use for testing faking internal types.")]
    internal class InternalSample
    {
        /// <summary>For testing.</summary>
        public string Data { get; set; }
    }
}
