using System.Collections;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

/// <summary>Verifies behavior.</summary>
public sealed class EnumerableCompareHintTests : CompareHintTestBase<EnumerableCompareHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly EnumerableCompareHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes
        = [typeof(IList), typeof(List<string>), typeof(List<KeyValuePair<int, int>>)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes
        = [typeof(object), typeof(int)];

    /// <summary>Sets up the tests.</summary>
    public EnumerableCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Theory, RandomData]
    internal void Compare_SizeMismatchOutOfBounds(List<string> original)
    {
        List<string> variant = Tools.Duplicator.Copy(original);
        variant.RemoveAt(variant.Count - 1);

        (bool, IEnumerable<Difference>) result = TestInstance.TryCompare(original, variant, CreateChainer());
        Tools.Asserter.Is(true, result.Item1);
        Tools.Asserter.HasCount(1, result.Item2);
        Tools.Asserter.Is(true, result.Item2.Single().ToString().Contains("outofbounds"));

        result = TestInstance.TryCompare(variant, original, CreateChainer());
        Tools.Asserter.Is(true, result.Item1);
        Tools.Asserter.HasCount(1, result.Item2);
        Tools.Asserter.Is(true, result.Item2.Single().ToString().Contains("outofbounds"));
    }
}
