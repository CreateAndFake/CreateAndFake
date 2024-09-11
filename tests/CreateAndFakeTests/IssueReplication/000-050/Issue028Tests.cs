namespace CreateAndFakeTests.IssueReplication;

public static class Issue028Tests
{
    internal sealed class Sample
    {
        public int Value { get; set; }
    }

    [Fact]
    internal static void Issue028_ConditionalCreation()
    {
        Tools.Randomizer.Create<Sample>(s => s.Value > 0).Value.Assert().GreaterThan(0);
    }
}