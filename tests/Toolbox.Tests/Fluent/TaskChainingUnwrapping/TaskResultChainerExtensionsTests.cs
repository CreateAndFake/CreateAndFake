using Werecodent.CreateAndFake.AsserterTool;

namespace Werecodent.CreateAndFake.Tests.Fluent.TaskChainingUnwrapping;

public static class TaskResultChainerExtensionsTests
{
    [Fact]
    internal static Task TaskResultChainerExtensions_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync(
            typeof(TaskResultChainerExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }

    [Fact]
    internal static Task TaskResultChainerExtensions_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync(
            typeof(TaskResultChainerExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }
}
