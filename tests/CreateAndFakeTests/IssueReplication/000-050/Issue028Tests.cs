using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue028Tests
    {
        internal class Sample
        {
            public int Value { get; set; }
        }

        [Fact]
        internal static void Issue028_ConditionalCreation()
        {
            Sample sample = Tools.Randomizer.Create<Sample>(s => s.Value > 0);
            (sample.Value > 0).Assert().Is(true);
        }
    }
}