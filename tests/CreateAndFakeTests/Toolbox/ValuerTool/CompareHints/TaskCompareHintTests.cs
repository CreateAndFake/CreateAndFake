using System;
using System.Collections;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Xunit;

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
        Tools.Asserter.IsNot(BuildExceptionTask(ex), BuildExceptionTask(Tools.Mutator.Variant(ex)));
    }

    private static Task BuildExceptionTask(Exception ex)
    {
        Task task = new(() => { throw ex; });
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
