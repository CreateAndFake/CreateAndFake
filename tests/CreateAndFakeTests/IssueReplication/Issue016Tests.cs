using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue016Tests
    {
        internal class Sample
        {
            public static int ValueA = 0;

            public int ValueB = 0;
        }

        [Theory, RandomData]
        internal static void Issue016_StaticsNotRandomized(Sample sample)
        {
            sample.ValueB.Assert().IsNot(0).Also(Sample.ValueA).Is(0);
        }
    }
}