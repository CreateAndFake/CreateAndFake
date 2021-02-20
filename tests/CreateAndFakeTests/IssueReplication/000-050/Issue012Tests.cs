using System.Diagnostics.CodeAnalysis;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue012Tests
    {
        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesVisible", Justification = "For testing.")]
        public abstract class Sample
        {
            /// <summary>For testing.</summary>
            public abstract double Value();
        }

        [Theory, RandomData]
        internal static void Issue012_RandomizedFake(Fake<Sample> sample)
        {
            sample.Dummy.Value().Assert().IsNot(sample.Dummy.Value());
        }
    }
}
