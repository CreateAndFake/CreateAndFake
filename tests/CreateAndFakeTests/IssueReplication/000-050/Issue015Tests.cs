using System;
using CreateAndFake;
using CreateAndFake.Design;
using Xunit;

namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue015Tests
{
    internal static class Sample
    {
        public static void Bad(int[] value)
        {
            ArgumentGuard.ThrowIfNull(value, nameof(value));

            value[0] = Tools.Mutator.Variant(value[0]);
        }
    }

    [Fact]
    internal static void Issue015_GuardsParameterMutation()
    {
        Tools.Asserter.Throws<AggregateException>(() =>
            Tools.Tester.PreventsParameterMutation(typeof(Sample)));
    }
}
