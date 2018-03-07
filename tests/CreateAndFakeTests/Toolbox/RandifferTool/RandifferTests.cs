using System;
using CreateAndFake;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandifferTool;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFakeTests.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandifferTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class RandifferTests
    {
        /// <summary>Verifies invalid nulls throw.</summary>
        [TestMethod]
        public void New_NullsThrow()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Randiffer(null, Tools.Valuer, Limiter.Few));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Randiffer(Tools.Randomizer, null, Limiter.Few));
        }

        /// <summary>Verifies null defaults to empty.</summary>
        [TestMethod]
        public void Create_AcceptsNull()
        {
            string value = Tools.Randomizer.Create<string>();

            Tools.Asserter.IsNot(null, Tools.Randiffer.Branch<string>(null));
            Tools.Asserter.IsNot(value, Tools.Randiffer.Branch(value, null));
        }

        /// <summary>Verifies extra values to not equal work.</summary>
        [TestMethod]
        public void Create_ManyValuesWorks()
        {
            string value1 = Tools.Randomizer.Create<string>();
            string value2 = Tools.Randomizer.Create<string>();
            string value3 = Tools.Randomizer.Create<string>();

            string result = Tools.Randiffer.Branch(value1, value2, value3);
            Tools.Asserter.IsNot(value1, result);
            Tools.Asserter.IsNot(value2, result);
            Tools.Asserter.IsNot(value3, result);
        }

        /// <summary>Verifies limiter limits attempts.</summary>
        [TestMethod]
        public void Create_TimesOut()
        {
            Fake<IValuer> fakeValuer = Tools.Faker.Mock<IValuer>();
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(true));

            Randiffer testInstance = new Randiffer(Tools.Randomizer, fakeValuer.Dummy, new Limiter(3));

            Tools.Asserter.Throws<TimeoutException>(
                () => testInstance.Branch(Tools.Randomizer.Create<DataSample>()));

            fakeValuer.VerifyTotalCalls(3);
        }

        /// <summary>Verifies create keeps trying until unequal instance is created.</summary>
        [TestMethod]
        public void Create_RepeatsUntilUnequal()
        {
            Fake<IValuer> fakeValuer = Tools.Faker.Mock<IValuer>();
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Series(true, true, true, false));

            Randiffer testInstance = new Randiffer(Tools.Randomizer, fakeValuer.Dummy, new Limiter(5));

            Tools.Asserter.IsNot(null, testInstance.Branch(Tools.Randomizer.Create<DataSample>()));
            fakeValuer.VerifyTotalCalls(4);
        }

        /// <summary>Verifies create keeps trying until unequal instance is created.</summary>
        [TestMethod]
        public void Create_RepeatsUntilBothUnequal()
        {
            Fake<IValuer> fakeValuer = Tools.Faker.Mock<IValuer>();
            fakeValuer.Setup(
                m => m.Equals(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Series(false, true, true, false, true, true, false, false));

            Randiffer testInstance = new Randiffer(Tools.Randomizer, fakeValuer.Dummy, new Limiter(5));

            Tools.Asserter.IsNot(null, testInstance.Branch(
                Tools.Randomizer.Create<DataSample>(),
                Tools.Randomizer.Create<DataSample>()));
            fakeValuer.VerifyTotalCalls(8);
        }
    }
}
