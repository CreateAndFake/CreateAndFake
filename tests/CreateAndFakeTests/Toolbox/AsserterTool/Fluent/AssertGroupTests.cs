using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

public static class AssertCollectionTests
{
    internal sealed class Item()
    {
        public int Number { get; set; }
        public string Text { get; set; }
    }

    [Fact]
    internal static void AssertGroup_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertGroup>();
    }

    [Fact]
    internal static void AssertGroup_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertGroup>();
    }

    [Theory, RandomData]
    internal static void IsEmpty_NoItems_Success([Size(0)] IEnumerable<Item> items)
    {
        items.Assert().IsEmpty();
    }

    [Theory, RandomData]
    internal static void IsEmpty_WithItems_Failure(IEnumerable<Item> items)
    {
        items.Assert(d => d.Assert().IsEmpty()).Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void IsNotEmpty_WithItems_Success(IEnumerable<Item> items)
    {
        items.Assert().IsNotEmpty();
    }

    [Theory, RandomData]
    internal static void IsNotEmpty_NoItems_Failure([Size(0)] IEnumerable<Item> items)
    {
        items.Assert(d => d.Assert().IsNotEmpty()).Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void HasCount_SameSize_Success(ICollection<Item> items)
    {
        items.Assert().HasCount(items.Count);
    }

    [Theory, RandomData]
    internal static void HasCount_MismatchedSize_Failure(ICollection<Item> items)
    {
        items.Assert(d => d.Assert().HasCount(Tools.Mutator.Variant(items.Count))).Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void Contains_UsingSubitem_Success(ICollection<Item> items)
    {
        items.Assert().Contains(Tools.Gen.NextItem(items));
    }

    [Theory, RandomData]
    internal static void Contains_RandomOther_Failure(ICollection<Item> items)
    {
        items
            .Assert(d => d.Assert().Contains(Tools.Mutator.Variant(items.First(), items.Skip(1).ToArray())))
            .Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void ContainsNot_RandomOther_Success(ICollection<Item> items)
    {
        items.Assert().ContainsNot(Tools.Mutator.Variant(items.First(), items.Skip(1).ToArray()));
    }

    [Theory, RandomData]
    internal static void ContainsNot_UsingSubitem_Failure(ICollection<Item> items)
    {
        items
            .Assert(d => d.Assert().ContainsNot(Tools.Gen.NextItem(items)))
            .Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void Fail_Throws_Success(IEnumerable<Item> items)
    {
        items.Assert(d => d.Assert().Fail()).Throws<AssertException>();
    }
}
