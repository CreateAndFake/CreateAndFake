using CreateAndFake;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.Design.Content
{
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

            Tools.Asserter.Is(1, comparer.GetHashCode(fake.Dummy));
            Tools.Asserter.Is(-1, comparer.GetHashCode(unequalFake.Dummy));

            fake.Setup(m => m.ValuesEqual(equalFake.Dummy), Behavior.Returns(true));
            Tools.Asserter.Is(true, comparer.Equals(fake.Dummy, equalFake.Dummy));

            fake.Setup(m => m.ValuesEqual(unequalFake.Dummy), Behavior.Returns(false));
            Tools.Asserter.Is(false, comparer.Equals(fake.Dummy, unequalFake.Dummy));

            fake.Setup(m => m.ValuesEqual(null), Behavior.Returns(false));
            Tools.Asserter.Is(false, comparer.Equals(fake.Dummy, null));
            Tools.Asserter.Is(false, comparer.Equals(null, fake.Dummy));
            Tools.Asserter.Is(true, comparer.Equals(null, null));

            Tools.Asserter.Is(0, comparer.Compare(fake.Dummy, fake.Dummy));
            Tools.Asserter.Is(0, comparer.Compare(fake.Dummy, equalFake.Dummy));
            Tools.Asserter.IsNot(0, comparer.Compare(fake.Dummy, unequalFake.Dummy));
            Tools.Asserter.IsNot(0, comparer.Compare(fake.Dummy, null));
            Tools.Asserter.IsNot(0, comparer.Compare(null, fake.Dummy));
            Tools.Asserter.Is(0, comparer.Compare(null, null));
        }
    }
}
