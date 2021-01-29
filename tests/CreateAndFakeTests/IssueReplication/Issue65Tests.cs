using System.Collections.Generic;
using CreateAndFake;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue65Tests
    {
        internal class InfiniteA
        {
            public ICollection<InfiniteB> Start { get; set; }
        }

        internal class InfiniteB
        {
            public InfiniteC Nested { get; set; }
        }

        internal class InfiniteC
        {
            public InfiniteA Back { get; set; }
        }

        [Theory, RandomData]
        internal static void Issue_HandlesInfiniteChainsA(InfiniteA sample)
        {
            TestInfiniteSample(sample);
        }

        [Theory, RandomData]
        internal static void Issue_HandlesInfiniteChainsB(InfiniteB sample)
        {
            TestInfiniteSample(sample);
        }

        [Theory, RandomData]
        internal static void Issue_HandlesInfiniteChainsC(InfiniteC sample)
        {
            TestInfiniteSample(sample);
        }

        private static void TestInfiniteSample<T>(T sample) where T: new()
        {
            Tools.Asserter.IsNot(null, sample);
            Tools.Asserter.IsNot(new T(), sample);
            Tools.Asserter.IsNot(sample, Tools.Mutator.Variant(sample));

            T dupe = Tools.Duplicator.Copy(sample);

            Tools.Asserter.Is(sample, dupe);
            Tools.Asserter.Is(Tools.Valuer.GetHashCode(sample), Tools.Valuer.GetHashCode(dupe));
        }
    }
}
