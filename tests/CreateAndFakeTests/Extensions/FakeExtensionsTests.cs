namespace CreateAndFakeTests.Extensions;

public static class FakeExtensionsTests
{
    [Fact]
    internal static void FakeExtensions_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException(typeof(FakeExtensions));
    }

    [Theory, RandomData]
    internal static void Called_PassesAsserters(int[] group, int comparable,
        string text, Action behavior, object item, [Fake] object data)
    {
        item.Assert().Called(data).And.Is(item);
        group.Assert().Called(data).And.Is(group);
        comparable.Assert().Called(data).And.Is(comparable);
        text.Assert().Called(data).And.Is(text);
        behavior.Assert().Called(data).And.Is(behavior);
    }
}