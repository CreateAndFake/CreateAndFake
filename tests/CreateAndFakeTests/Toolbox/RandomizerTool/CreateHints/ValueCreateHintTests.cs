using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

/// <summary>Verifies behavior.</summary>
public sealed class ValueCreateHintTests : CreateHintTestBase<ValueCreateHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly ValueCreateHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = [.. ValueRandom.ValueTypes];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public ValueCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
}
