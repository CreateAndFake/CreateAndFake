using System;
using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class RandomizerTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void Randomizer_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<Randomizer>();
        }

        /// <summary>Verifies an exception throws when no hint matches.</summary>
        [Fact]
        public static void Create_NoRulesThrows()
        {
            Tools.Asserter.Throws<NotSupportedException>(
                () => new Randomizer(Tools.Faker, new FastRandom(), false).Create<object>());
        }

        /// <summary>Verifies hint behavior works.</summary>
        [Fact]
        public static void Create_MissingMatchThrows()
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

        /// <summary>Verifies hint behavior works.</summary>
        [Fact]
        public static void Create_ValidHintWorks()
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
