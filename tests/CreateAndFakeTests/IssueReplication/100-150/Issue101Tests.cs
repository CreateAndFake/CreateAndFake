namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue101Tests
{
    [Theory, RandomData]
    internal static void Issue101_EasyCloneValues(int value)
    {
        value.CreateDeepClone().Assert().Is(value);
    }

    [Theory, RandomData]
    internal static void Issue101_EasyCloneObjects(string value)
    {
        value.CreateDeepClone().Assert().Is(value);
    }
}
