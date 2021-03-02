using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue086Tests
    {
        [Theory, RandomData]
        internal static void Issue086_SizeWorks([Size(5)] string[] items)
        {
            items.Length.Assert().Is(5);
        }

        [Theory, RandomData]
        internal static void Issue086_EmptyCollection([Size(0)] List<int> items)
        {
            items.Any().Assert().Is(false);
        }

        [Theory, RandomData]
        internal static void Issue086_LegacyCollection([Size(3)] Hashtable items)
        {
            items.Keys.Count.Assert().Is(3);
        }
    }
}