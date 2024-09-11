using CreateAndFake.Design;

namespace CreateAndFakeTests.IssueReplication;

public static class Issue015Tests
{
    internal static class Sample
    {
        public static void Bad(int[] value)
        {
            ArgumentGuard.ThrowIfNull(value, nameof(value));

            value[0] = value[0].CreateVariant();
        }
    }

    [Fact]
    internal static void Issue015_GuardsParameterMutation()
    {
        typeof(Sample).Assert(t => Tools.Tester.PreventsParameterMutation(t)).Throws<AggregateException>();
    }
}
