using System.Collections.Generic;
using CreateAndFake;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue096Tests
    {
        [Fact]
        internal static void Issue096_SupportsIAsyncEnumerable()
        {
            TestSample<IAsyncEnumerable<int>>();
            TestSample<IAsyncEnumerable<string>>();
            TestSample<IAsyncEnumerator<object>>();
        }

        private static void TestSample<T>()
        {
            for (int i = 0; i < 50; i++)
            {
                T sample = Tools.Randomizer.Create<T>();
                Tools.Asserter.IsNot(null, sample);
                Tools.Asserter.IsNot(sample, Tools.Mutator.Variant(sample));

                T dupe = Tools.Duplicator.Copy(sample);

                Tools.Asserter.Is(sample, dupe);
                Tools.Asserter.Is(Tools.Valuer.GetHashCode(sample), Tools.Valuer.GetHashCode(dupe));
            }
        }
    }
}
