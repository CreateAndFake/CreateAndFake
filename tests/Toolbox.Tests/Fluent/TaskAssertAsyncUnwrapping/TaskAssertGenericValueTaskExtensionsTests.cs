using System.Reflection;
using Werecodent.CreateAndFake.AsserterTool;
using Werecodent.CreateAndFake.Design.Exceptions;
using Werecodent.CreateAndFake.Fluent.AssertAsyncCalls;

namespace Werecodent.CreateAndFake.Tests.Fluent.TaskAssertAsyncUnwrapping;

public static class TaskAssertGenericValueTaskExtensionsTests
{
    [Fact]
    internal static Task TaskAssertGenericValueTaskExtensions_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync(
            typeof(TaskAssertGenericValueTaskExtensions),
            TestContext.Current.CancellationToken,
            opt =>
                opt with
                {
                    IgnorableExceptions =
                    [
                        typeof(AssertException),
                        typeof(ValueTaskRepeatedAccessException),
                    ],
                }
        );
    }

    [Fact]
    internal static Task TaskAssertGenericValueTaskExtensions_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync(
            typeof(TaskAssertGenericValueTaskExtensions),
            TestContext.Current.CancellationToken,
            opt =>
                opt with
                {
                    IgnorableExceptions =
                    [
                        typeof(AssertException),
                        typeof(ValueTaskRepeatedAccessException),
                    ],
                }
        );
    }

    [Fact]
    internal static void TaskAssertGenericValueTaskExtensions_MatchesEveryMethod()
    {
        typeof(AssertGenericValueTaskBase<,>)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .OrderBy(m => m.Name)
            .Select(m => m.Name)
            .Where(m => m != nameof(AssertGenericTaskBase<,>.ThrowsAsync))
            .Where(m => m != nameof(AssertGenericTaskBase<,>.ThrowsNoAsync))
            .Assert()
            .Is(
                typeof(TaskAssertGenericValueTaskExtensions)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .OrderBy(m => m.Name)
                    .Select(m => m.Name)
                    .Where(m =>
                        m != nameof(TaskAssertGenericValueTaskExtensions.ThrowsExceptionAsync)
                    )
                    .Where(m =>
                        m != nameof(TaskAssertGenericValueTaskExtensions.ThrowsNoExceptionAsync)
                    )
            );
    }
}
