using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue078Tests
    {
        [Theory, RandomData]
        internal static void Issue_CanChainObjects(int[] valueA, bool valueB)
        {
            valueA.Assert().IsNotEmpty().Also(valueB).IsNot(null);
        }

        [Theory, RandomData]
        internal static void Issue_CanChainCollections(bool valueA, int[] valueB)
        {
            valueA.Assert().IsNot(null).Also(valueB).IsNotEmpty();
        }
    }
}