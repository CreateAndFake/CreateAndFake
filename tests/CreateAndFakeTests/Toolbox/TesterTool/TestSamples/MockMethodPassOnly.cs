using System.Diagnostics.CodeAnalysis;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples
{
    /// <summary>For testing.</summary>
    public sealed class MockMethodPassOnly
    {
        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For testing.")]
        public bool MockPassOnly(IOnlyMockSample sample)
        {
            sample?.FailIfNotMocked();
            return true;
        }
    }
}
