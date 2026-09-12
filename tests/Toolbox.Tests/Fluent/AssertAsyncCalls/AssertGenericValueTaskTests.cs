using Werecodent.CreateAndFake.AsserterTool;
using Werecodent.CreateAndFake.Design.Exceptions;
using Werecodent.CreateAndFake.Fluent.AssertAsyncCalls;

namespace Werecodent.CreateAndFake.Tests.Fluent.AssertAsyncCalls;

public static class AssertGenericValueTaskTests
{
    private static readonly TesterMod _Config = opt =>
        opt with
        {
            IgnorableExceptions =
            [
                typeof(AssertException),
                typeof(ToolException),
                typeof(InvalidCastException),
                typeof(ArgumentException),
                typeof(ValueTaskRepeatedAccessException),
            ],
        };

    [Fact]
    internal static Task AssertGenericValueTask_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync<AssertGenericValueTask<string>>(
            TestContext.Current.CancellationToken,
            _Config
        );
    }

    [Fact]
    internal static Task AssertGenericValueTask_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync<AssertGenericValueTask<string>>(
            TestContext.Current.CancellationToken,
            _Config
        );
    }

    /*[Theory, RandomData]
    internal static async Task AssertGenericValueTask_CallsAndChains(
        Injected<AssertGenericValueTask<DataSample>> instance
    )
    {
        RunResults results = await Tools.Runner.CallMethodsOnAsync(
            instance.Dummy,
            TestContext.Current.CancellationToken,
            opt => opt with { IncludeBaseObjectMethods = false }
        );
        results
            .RawResults.Where(r => r.Result != null)
            .Where(r => r.Result is not AssertChainer<AssertGenericValueTask<DataSample>>)
            .Where(r => r.Result is not AlsoChainer)
            .Where(r => r.Result.GetType().Inherits(typeof(ExceptionChainer<>)))
            .Where(r => r.Result as string != "AssertGenericValueTask<DataSample>")
            .Assert()
            .IsEmpty();
    }*/
}
