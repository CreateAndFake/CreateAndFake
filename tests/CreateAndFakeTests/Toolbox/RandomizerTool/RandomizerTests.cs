using System;
using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class RandomizerTests
    {
        /// <summary>Verifies nulls are valid.</summary>
        [TestMethod]
        public void New_NullHintsValid()
        {
            Tools.Asserter.IsNot(null, new Randomizer(Tools.Faker, new FastRandom(), true, null));
            Tools.Asserter.IsNot(null, new Randomizer(Tools.Faker, new FastRandom(), false, null));
        }

        /// <summary>Verifies random must be provided.</summary>
        [TestMethod]
        public void New_NullsThrows()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Randomizer(Tools.Faker, null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Randomizer(null, new FastRandom()));
        }

        /// <summary>Verifies an exception throws when no hint matches.</summary>
        [TestMethod]
        public void Create_NoRulesThrows()
        {
            Tools.Asserter.Throws<NotSupportedException>(
                () => new Randomizer(Tools.Faker, new FastRandom(), false).Create<object>());
        }

        /// <summary>Verifies hint behavior works.</summary>
        [TestMethod]
        public void Create_MissingMatchThrows()
        {
            string data = "Result";

            Fake<CreateHint> hint = Tools.Faker.Mock<CreateHint>();
            hint.Setup(
                m => m.TryCreate(data.GetType(), Arg.Any<RandomizerChainer>()),
                Behavior.Returns((false, (object)null), Times.Once));

            Tools.Asserter.Throws<NotSupportedException>(
                () => new Randomizer(Tools.Faker, new FastRandom(), false, hint.Dummy).Create<string>());

            hint.Verify(Times.Once);
        }

        /// <summary>Verifies null type can't be created.</summary>
        [TestMethod]
        public void Create_NullTypeThrows()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new Randomizer(Tools.Faker, new FastRandom()).Create(null));
        }

        /// <summary>Verifies hint behavior works.</summary>
        [TestMethod]
        public void Create_ValidHintWorks()
        {
            string data = "Result";

            Fake<CreateHint> hint = Tools.Faker.Mock<CreateHint>();
            hint.Setup(
                m => m.TryCreate(data.GetType(), Arg.Any<RandomizerChainer>()),
                Behavior.Returns((true, (object)data), Times.Once));

            Tools.Asserter.Is(data, new Randomizer(Tools.Faker,
                new FastRandom(), false, hint.Dummy).Create<string>());

            hint.Verify(Times.Once);
        }
    }
}
