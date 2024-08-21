using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

/// <summary>Verifies behavior.</summary>
public sealed class TaskCreateHintTests : CreateHintTestBase<TaskCreateHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly TaskCreateHint _TestInstance = new();

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
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public TaskCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
}
