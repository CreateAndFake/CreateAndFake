using CreateAndFake;
using CreateAndFake.Design.Data;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue009Tests
{
    internal sealed class Sample(string lastName)
    {
        public string FirstName { get; set; }

        public string _middleName = null;

        public string LastName { get; } = lastName;
    }

    [Theory, RandomData]
    internal static void Issue009_SmartDataIsUsed(Sample sample)
    {
        NameData.Values.Contains(sample.FirstName).Assert().Is(true);
        NameData.Values.Contains(sample._middleName).Assert().Is(true);
        NameData.Values.Contains(sample.LastName).Assert().Is(true);
    }
}