using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue018Tests
    {
        internal class Sample
        {
            public IInternalSample InternalSample { get; }

            public Sample(IInternalSample internalSample)
            {
                InternalSample = internalSample;
            }
        }

        public interface IInternalSample
        {
            string Value { get; }
        }

        [Theory, RandomData]
        internal static void Issue018_RandomizerInjects(Injected<Sample> sample)
        {
            sample.Dummy.InternalSample.Assert().Is(sample.Fake<IInternalSample>().Dummy);
        }
    }
}