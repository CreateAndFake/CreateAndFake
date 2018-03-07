using CreateAndFake;
using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.FakerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Design.Content
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class ValueComparer_T_Tests
    {
        /// <summary>Verifies the type works as intended.</summary>
        [TestMethod]
        public void ValueComparer_ValueEquatableBehavior()
        {
            ValueComparer<IValueEquatable> comparer = ValueComparer<IValueEquatable>.Use;

            Fake<IValueEquatable> fake = Tools.Faker.Mock<IValueEquatable>();
            Fake<IValueEquatable> equalFake = Tools.Faker.Mock<IValueEquatable>();
            Fake<IValueEquatable> unequalFake = Tools.Faker.Mock<IValueEquatable>();

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
