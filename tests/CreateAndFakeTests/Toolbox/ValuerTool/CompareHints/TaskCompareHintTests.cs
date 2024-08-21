using System.Collections;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

#pragma warning disable CA1849 // Task await synchronously blocks

/// <summary>Verifies behavior.</summary>
public sealed class TaskCompareHintTests : CompareHintTestBase<TaskCompareHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly TaskCompareHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes =
    [
        typeof(Task<DataHolderSample>),
        typeof(Task<object>),
        typeof(Task<string>),
        typeof(Task<int>),
        typeof(Task<bool>),
        typeof(Task)
    ];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(IEnumerable), typeof(string), typeof(int)];

    /// <summary>Sets up the tests.</summary>
    public TaskCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Theory, RandomData]
    internal void Compare_NonGenericTasksCompareByException(Exception ex)
    {
        BuildTask(ex)
            .Assert()
            .IsNot(BuildTask(Tools.Mutator.Variant(ex)))
            .And
            .IsNot(BuildTask(null));
    }

    private static Task BuildTask(Exception ex)
    {
        Task task = new(() =>
        {
            if (ex != null)
            {
                throw ex;
            }
        });
        try
        {
            task.Start();
            task.Wait();
        }
        catch (AggregateException)
        {
            // Throw intentional.
        }
        return task;
    }
}

#pragma warning restore CA1849 // Task await synchronously blocks
