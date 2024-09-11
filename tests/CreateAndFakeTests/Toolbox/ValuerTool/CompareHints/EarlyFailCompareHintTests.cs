using System.Collections;
using System.Reflection;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

public sealed class EarlyFailCompareHintTests : CompareHintTestBase<EarlyFailCompareHint>
{
    private static readonly EarlyFailCompareHint _TestInstance = new();

    private static readonly Type[] _ValidTypes =
    [
        typeof(int),
        typeof(string),
        typeof(BindingFlags),
        typeof(Type),
        typeof(Delegate)
    ];

    private static readonly Type[] _InvalidTypes =
    [
        typeof(IDictionary),
        typeof(IEnumerable),
        typeof(IAsyncEnumerable<int>)
    ];

    public EarlyFailCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Fact]
    internal void TryCompare_NullBehaviorCheck()
    {
        TestInstance.TryCompare(null, new object(), CreateChainer()).Item1.Assert().Is(true);
        TestInstance.TryCompare(null, new object(), CreateChainer()).Item2.Assert().IsNotEmpty();

        TestInstance.TryCompare(null, null, CreateChainer()).Item1.Assert().Is(true);
        TestInstance.TryCompare(null, null, CreateChainer()).Item2.Assert().IsEmpty();

        TestInstance.TryCompare(new object(), null, CreateChainer()).Item1.Assert().Is(true);
        TestInstance.TryCompare(new object(), null, CreateChainer()).Item2.Assert().IsNotEmpty();
    }

    [Fact]
    internal void TryGetHashCode_NullBehaviorCheck()
    {
        TestInstance.TryGetHashCode(null, CreateChainer()).Assert().Is((true, ValueComparer.NullHash));
    }

    [Theory, RandomData]
    internal void TryCompare_MismatchedTypesDifferent(int value)
    {
        TestInstance.TryCompare(value, new object(), CreateChainer()).Item2.Assert().IsNotEmpty();
    }
}
