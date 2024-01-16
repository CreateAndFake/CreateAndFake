using System;
using System.Collections;
using System.Linq;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Verifies behavior.</summary>
public sealed class LegacyCollectionCopyHintTests : CopyHintTestBase<LegacyCollectionCopyHint>
{
    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = LegacyCollectionCreateHint.PotentialCollections
        .Except(new[] { typeof(ArrayList), typeof(Queue), typeof(Stack), typeof(Array) }).ToArray();

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public LegacyCollectionCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }
}
