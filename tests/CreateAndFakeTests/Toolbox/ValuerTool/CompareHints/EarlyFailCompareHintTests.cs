using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Design.Content;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.ValuerTool.CompareHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool.CompareHints;

/// <summary>Verifies behavior.</summary>
public sealed class EarlyFailCompareHintTests : CompareHintTestBase<EarlyFailCompareHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly EarlyFailCompareHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes
        = [typeof(int), typeof(string), typeof(BindingFlags), typeof(Type), typeof(Delegate)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes
        = [typeof(IDictionary), typeof(IEnumerable), typeof(IAsyncEnumerable<int>)];

    /// <summary>Sets up the tests.</summary>
    public EarlyFailCompareHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Fact]
    internal void TryCompare_NullBehaviorCheck()
    {
        Tools.Asserter.Is(true, TestInstance.TryCompare(null, new object(), CreateChainer()).Item1);
        Tools.Asserter.IsNotEmpty(TestInstance.TryCompare(null, new object(), CreateChainer()).Item2.ToArray());

        Tools.Asserter.Is(true, TestInstance.TryCompare(null, null, CreateChainer()).Item1);
        Tools.Asserter.IsEmpty(TestInstance.TryCompare(null, null, CreateChainer()).Item2);

        Tools.Asserter.Is(true, TestInstance.TryCompare(new object(), null, CreateChainer()).Item1);
        Tools.Asserter.IsNotEmpty(TestInstance.TryCompare(
            new object(), null, CreateChainer()).Item2.ToArray());
    }

    [Fact]
    internal void TryGetHashCode_NullBehaviorCheck()
    {
        Tools.Asserter.Is(true, TestInstance.TryGetHashCode(null, CreateChainer()).Item1);
        Tools.Asserter.Is(ValueComparer.NullHash, TestInstance.TryGetHashCode(null, CreateChainer()).Item2);
    }

    [Theory, RandomData]
    internal void TryCompare_MismatchedTypesDifferent(int value)
    {
        TestInstance.TryCompare(value, new object(), CreateChainer()).Item2.ToArray().Assert().IsNotEmpty();
    }
}
