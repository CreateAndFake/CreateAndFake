using Werecodent.CreateAndFake.Fluent.Chaining;

namespace Werecodent.CreateAndFake.Tests.Fluent.Chaining;

public static class ResultChainerTests
{
    [Fact]
    internal static Task ResultChainer_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync(
            typeof(ResultChainer<>),
            TestContext.Current.CancellationToken
        );
    }

    [Fact]
    internal static Task ResultChainer_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync(
            typeof(ResultChainer<>),
            TestContext.Current.CancellationToken
        );
    }
}
