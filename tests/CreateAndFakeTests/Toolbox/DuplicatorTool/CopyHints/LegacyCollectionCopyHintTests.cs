﻿using System.Collections;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

public sealed class LegacyCollectionCopyHintTests : CopyHintTestBase<LegacyCollectionCopyHint>
{
    private static readonly Type[] _ValidTypes = LegacyCollectionCreateHint.PotentialCollections
        .Except([typeof(ArrayList), typeof(Queue), typeof(Stack), typeof(Array)])
        .ToArray();

    private static readonly Type[] _InvalidTypes = [typeof(object)];

    public LegacyCollectionCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }
}
