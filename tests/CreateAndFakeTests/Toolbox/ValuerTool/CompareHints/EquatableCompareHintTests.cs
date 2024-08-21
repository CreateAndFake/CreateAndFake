using System.Collections;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

/// <summary>Verifies behavior.</summary>
public sealed class EquatableCompareHintTests : CompareHintTestBase<EquatableCompareHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly EquatableCompareHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = [typeof(Limiter)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(IDictionary), typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public EquatableCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
}
