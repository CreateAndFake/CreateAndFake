using System.Collections;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

/// <summary>Verifies behavior.</summary>
public sealed class AsyncEnumerableCompareHintTests : CompareHintTestBase<AsyncEnumerableCompareHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly AsyncEnumerableCompareHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes
        = [typeof(IAsyncEnumerable<int>), typeof(IAsyncEnumerable<string>), typeof(IAsyncEnumerable<object>)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(IEnumerable), typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public AsyncEnumerableCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Theory, RandomData]
    internal void Supports_NullTest(IAsyncEnumerable<string> data)
    {
        TestInstance.TryCompare(null, data, CreateChainer()).Item1.Assert().Is(false);
        TestInstance.TryCompare(data, null, CreateChainer()).Item1.Assert().Is(false);
    }
}
