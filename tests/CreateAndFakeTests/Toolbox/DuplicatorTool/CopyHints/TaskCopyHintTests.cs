using System;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Verifies behavior.</summary>
public sealed class TaskCopyHintTests : CopyHintTestBase<TaskCopyHint>
{
    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes =
    [
        typeof(Task<DataHolderSample>),
        typeof(Task<object>),
        typeof(Task<string>),
        typeof(Task<int>),
        typeof(Task<bool>)
    ];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public TaskCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }

    [Fact]
    internal void TryCopy_NonGenericTaskFalse()
    {
        Tools.Asserter.Is((false, (object)null), TestInstance.TryCopy(new Task(() => { }), CreateChainer()));
    }
}
