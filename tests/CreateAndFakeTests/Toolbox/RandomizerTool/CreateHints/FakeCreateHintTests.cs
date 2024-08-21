using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

/// <summary>Verifies behavior.</summary>
public sealed class FakeCreateHintTests : CreateHintTestBase<FakeCreateHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly FakeCreateHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes
        = [typeof(Fake<object>), typeof(Fake<OutSample>), typeof(Fake<GenericSample<string>>)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public FakeCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
}
