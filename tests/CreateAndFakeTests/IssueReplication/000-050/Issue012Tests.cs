using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue012Tests
{
    /// <summary>For testing.</summary>
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
