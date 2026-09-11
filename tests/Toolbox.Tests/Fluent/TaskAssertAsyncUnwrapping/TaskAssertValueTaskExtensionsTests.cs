using Werecodent.CreateAndFake.AsserterTool;

namespace Werecodent.CreateAndFake.Tests.Fluent.TaskAssertAsyncUnwrapping;

public static class TaskAssertValueTaskExtensionsTests
{
    [Fact]
    internal static Task TaskAssertValueTaskExtensions_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync(
            typeof(TaskAssertValueTaskExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }

    [Fact]
    internal static Task TaskAssertValueTaskExtensions_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync(
            typeof(TaskAssertValueTaskExtensions),
            TestContext.Current.CancellationToken,
            opt => opt with { IgnorableExceptions = [typeof(AssertException)] }
        );
    }
}
