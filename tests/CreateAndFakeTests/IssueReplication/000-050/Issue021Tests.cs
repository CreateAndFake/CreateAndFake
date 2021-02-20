using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue021Tests
    {
        internal class Sample
        {
            public string Value { get; set; }
        }

        [Theory, RandomData]
        internal static void Issue021_MutatorMutates(Sample sample)
        {
            string original = sample.Value;
            Tools.Mutator.Modify(sample);
            sample.Value.Assert().IsNot(original);
        }
    }
}