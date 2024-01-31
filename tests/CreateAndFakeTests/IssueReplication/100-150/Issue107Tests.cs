using System;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue107Tests
{
    internal abstract class SelfReferenceContainer<T> where T : SelfReferenceContainer<T>
    {
        public T Content { get; set; }
    }

    [Fact]
    internal static void Issue107_SelfReferenceGenericMustHaveSubclass()
    {
        typeof(SelfReferenceContainer<>).Assert(c => c.CreateRandomInstance()).Throws<InvalidOperationException>();
    }
}