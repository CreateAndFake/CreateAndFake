using Werecodent.CreateAndFake.AsserterTool;
using Werecodent.CreateAndFake.Design.Exceptions;
using Werecodent.CreateAndFake.FakerTool;
using Werecodent.CreateAndFake.Fluent.AssertAsyncCalls;
using Werecodent.CreateAndFake.Fluent.Chaining;
using Werecodent.CreateAndFake.RunnerTool;

namespace Werecodent.CreateAndFake.Tests.Fluent.AssertAsyncCalls;

public static class AssertAsyncObjectTests
{
    [Fact]
    internal static Task AssertAsyncObject_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync<AssertAsyncObject>(
            TestContext.Current.CancellationToken,
            opt =>
                opt with
                {
                    IgnorableExceptions = [typeof(AssertException), typeof(ToolException)],
                }
        );
    }

    [Fact]
    internal static Task AssertAsyncObject_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync<AssertAsyncObject>(
            TestContext.Current.CancellationToken,
            opt =>
                opt with
                {
                    IgnorableExceptions = [typeof(AssertException), typeof(ToolException)],
                }
        );
    }

    [Theory, RandomData]
    internal static async Task AssertAsyncObject_CallsAndChains(
        Injected<AssertAsyncObject> instance
    )
    {
        RunResults results = await Tools.Runner.CallMethodsOnAsync(
            instance.Dummy,
            TestContext.Current.CancellationToken,
            opt => opt with { IncludeBaseObjectMethods = false }
        );
        results
            .RawResults.Where(r => r.Result != null)
            .Where(r => r.Result is not AssertChainer<AssertAsyncObject>)
            .Where(r => r.Result as string != nameof(AssertAsyncObject))
            .Assert()
            .IsEmpty();
    }
}
