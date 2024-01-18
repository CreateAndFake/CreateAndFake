using System.Collections.Generic;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.AsserterTool;
using Xunit;

namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue106Tests
{
    internal sealed class RandomNameContainer<T>
    {
        public T Content { get; set; }
    }

    public sealed class RandomNameItem
    {
        public string Message { get; set; }
    }

    [Theory, RandomData]
    internal static void Issue106_AssertIncludesGenericName(RandomNameContainer<RandomNameItem> generic)
    {
        generic
            .Assert(l => l.Assert().Is(generic.CreateVariant()))
            .Throws<AssertException>()
            .Message
            .Assert()
            .Contains(nameof(RandomNameItem));
    }

    [Theory, RandomData]
    internal static void Issue106_AssertIncludesGenericNameCollections(List<Dictionary<string, RandomNameItem>> generic)
    {
        generic
            .Assert(l => l.Assert().Is(generic.CreateVariant()))
            .Throws<AssertException>()
            .Message
            .Assert()
            .Contains(nameof(RandomNameItem));
    }
}
