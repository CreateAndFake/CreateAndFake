using System;
using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue109Tests
{
    [Theory, RandomData]
    internal static void Issue109_BinarySerializationRemoved(DateTime date)
    {
        date.CreateDeepClone().Assert().Is(date);
    }
}
