﻿using System.Collections;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

public sealed class StatelessCompareHintTests : CompareHintTestBase<StatelessCompareHint>
{
    private static readonly StatelessCompareHint _TestInstance = new();

    private static readonly Type[] _ValidTypes = [typeof(StatelessSample)];

    private static readonly Type[] _InvalidTypes = [typeof(object), typeof(string), typeof(IList), typeof(int)];

    public StatelessCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    public override void TryCompare_SupportsDifferentValidTypes()
    {
        // Stateless objects can't be different.
    }
}
