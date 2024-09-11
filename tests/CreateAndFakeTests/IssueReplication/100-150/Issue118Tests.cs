using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.IssueReplication;

public static class Issue118Tests
{
    [Fact]
    internal static void Issue118_AddsPass()
    {
        typeof(object).Assert().Pass();
    }

    [Theory, RandomData]
    internal static void Issue118_FixesFakeTypeRandomData(Fake<Type> type)
    {
        type.Assert().IsNot(null);
    }

#if !LEGACY
    [Theory, RandomData]
    internal static void Issue118_FixesStubTypeRandomData([Stub] Type type)
    {
        type.Assert().IsNot(null);
    }
#endif
}
