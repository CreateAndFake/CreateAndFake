using System.Reflection;
using Werecodent.CreateAndFake.AsserterTool;
using Werecodent.CreateAndFake.Design.Exceptions;
using Werecodent.CreateAndFake.FakerTool;
using Werecodent.CreateAndFake.Fluent.AssertCalls;
using Werecodent.CreateAndFake.Fluent.Chaining;
using Werecodent.CreateAndFake.RunnerTool;

namespace Werecodent.CreateAndFake.Tests.Fluent.AssertCalls;

public static class AssertStringTests
{
    private static readonly TesterMod _Config = opt =>
        opt with
        {
            IgnorableExceptions =
            [
                typeof(AssertException),
                typeof(ToolException),
                typeof(InvalidCastException),
                typeof(UnsupportedException),
                typeof(TargetException),
            ],
        };

    [Fact]
    internal static Task AssertString_GuardsNulls()
    {
        return Tools.Tester.PreventsNullRefExceptionAsync<AssertString>(
            TestContext.Current.CancellationToken,
            _Config
        );
    }

    [Fact]
    internal static Task AssertString_NoParameterMutation()
    {
        return Tools.Tester.PreventsParameterMutationAsync<AssertString>(
            TestContext.Current.CancellationToken,
            _Config
        );
    }

    [Theory, RandomData]
    internal static void AssertString_FullSupport(
        [Size(5)] string original,
        [Copy] string clone,
        string variant
    )
    {
        original
            .Assert()
            .Pass()
            .And()
            .Is(clone)
            .And()
            .IsNot(variant)
            .And()
            .IsNotEmpty()
            .And()
            .IsNotNull()
            .And()
            .ReferenceEqual(original)
            .And()
            .ReferenceNotEqual(variant)
            .And()
            .HasCount(5)
            .And()
            .HasCountLessOrExactly(5)
            .And()
            .HasCountLessOrExactly(6)
            .And()
            .HasCountLessThan(6)
            .And()
            .HasCountMoreOrExactly(5)
            .And()
            .HasCountMoreOrExactly(4)
            .And()
            .HasCountMoreThan(4)
            .And()
            .Contains(original[0])
            .And()
            .StartsWith($"{original[0]}");
    }

    [Theory, RandomData]
    internal static async Task AssertString_CallsAndChains(Injected<AssertString> instance)
    {
        RunResults results = await Tools.Runner.CallMethodsOnAsync(
            instance.Dummy,
            TestContext.Current.CancellationToken
        );
        results
            .RawResults.Where(r => r.Result != null)
            .Where(r =>
                r.Result
                    is not AssertChainer<AssertString>
                        and Task<AssertChainer<AssertEnumerable>>
            )
            .Assert()
            .IsEmpty();
    }
}
