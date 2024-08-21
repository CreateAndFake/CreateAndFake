using CreateAndFake;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using CreateAndFake.Fluent;

namespace CreateAndFakeTests.Design.Content;

/// <summary>Verifies behavior.</summary>
public static class ValueComparer_T_Tests
{
    [Fact]
    internal static void ValueComparer_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<ValueComparer<IValueEquatable>>();
    }

    [Fact]
    internal static void ValueComparer_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<ValueComparer<IValueEquatable>>();
    }

    [Theory, RandomData]
    internal static void ValueComparer_ValueEquatableBehavior(
        Fake<IValueEquatable> fake,
        Fake<IValueEquatable> equalFake,
        Fake<IValueEquatable> unequalFake)
    {
        ValueComparer<IValueEquatable> comparer = ValueComparer<IValueEquatable>.Use;

        fake.Setup(m => m.GetValueHash(), Behavior.Returns(1));
        equalFake.Setup(m => m.GetValueHash(), Behavior.Returns(1));
        unequalFake.Setup(m => m.GetValueHash(), Behavior.Returns(-1));

        comparer.GetHashCode(fake.Dummy).Assert().Is(1);
        comparer.GetHashCode(unequalFake.Dummy).Assert().Is(-1);
        comparer.GetHashCode([fake.Dummy]).Assert().IsNot(0);
        comparer.GetHashCode(MapByIndex([fake.Dummy])).Assert().IsNot(0);

        fake.Setup(m => m.ValuesEqual(equalFake.Dummy), Behavior.Returns(true));
        comparer.Equals(fake.Dummy, equalFake.Dummy).Assert().Is(true);
        comparer.Equals([fake.Dummy], [fake.Dummy]).Assert().Is(true);
        comparer.Equals(MapByIndex([fake.Dummy]), MapByIndex([fake.Dummy])).Assert().Is(true);

        fake.Setup(m => m.ValuesEqual(unequalFake.Dummy), Behavior.Returns(false));
        comparer.Equals(fake.Dummy, unequalFake.Dummy).Assert().Is(false);
        comparer.Equals([fake.Dummy], [unequalFake.Dummy]).Assert().Is(false);
        comparer.Equals(MapByIndex([fake.Dummy]), MapByIndex([unequalFake.Dummy])).Assert().Is(false);

        fake.Setup(m => m.ValuesEqual(null), Behavior.Returns(false));
        comparer.Equals(fake.Dummy, null).Assert().Is(false);
        comparer.Equals(null, fake.Dummy).Assert().Is(false);
        comparer.Equals((IValueEquatable)null, null).Assert().Is(true);

        comparer.Compare(fake.Dummy, fake.Dummy).Assert().Is(0);
        comparer.Compare(fake.Dummy, equalFake.Dummy).Assert().Is(0);
        comparer.Compare(fake.Dummy, unequalFake.Dummy).Assert().IsNot(0);
        comparer.Compare(fake.Dummy, null).Assert().IsNot(0);
        comparer.Compare(null, fake.Dummy).Assert().IsNot(0);
        comparer.Compare((IValueEquatable)null, null).Assert().Is(0);
    }

    private static Dictionary<int, T> MapByIndex<T>(IEnumerable<T> collection)
    {
        return collection
            .Select((item, index) => new { index, item })
            .ToDictionary(pair => pair.index, pair => pair.item);
    }
}
