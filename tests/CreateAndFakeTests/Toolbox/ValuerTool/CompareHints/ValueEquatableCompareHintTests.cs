﻿using System.Collections;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

public sealed class ValueEquatableCompareHintTests : CompareHintTestBase<ValueEquatableCompareHint>
{
    private static readonly ValueEquatableCompareHint _TestInstance = new();

    private static readonly Type[] _ValidTypes = [typeof(IValueEquatable), typeof(ValueEquatableSample)];

    private static readonly Type[] _InvalidTypes = [typeof(IDictionary), typeof(object)];

    public ValueEquatableCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
}
