using System.Collections;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

/// <summary>Verifies behavior.</summary>
public sealed class ValueEquatableCompareHintTests : CompareHintTestBase<ValueEquatableCompareHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly ValueEquatableCompareHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = [typeof(IValueEquatable), typeof(ValueEquatableSample)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(IDictionary), typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public ValueEquatableCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
}
