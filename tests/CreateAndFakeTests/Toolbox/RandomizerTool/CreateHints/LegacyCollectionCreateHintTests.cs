using System.Collections;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

/// <summary>Verifies behavior.</summary>
public sealed class LegacyCollectionCreateHintTests : CreateHintTestBase<LegacyCollectionCreateHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly LegacyCollectionCreateHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = LegacyCollectionCreateHint.PotentialCollections
        .Concat(new[] { typeof(IEnumerable), typeof(IList), typeof(IDictionary) }).ToArray();

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public LegacyCollectionCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
}
