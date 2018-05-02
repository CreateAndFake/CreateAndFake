﻿using System;
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
            Tools.Tester.PreventsNullRefException(Tools.Randomizer);
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void Randomizer_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(Tools.Randomizer);
        }

        /// <summary>Verifies nulls are valid.</summary>
        [Fact]
        public static void New_NullHintsValid()
        {
            Tools.Asserter.IsNot(null, new Randomizer(Tools.Faker, new FastRandom(), true, null));
            Tools.Asserter.IsNot(null, new Randomizer(Tools.Faker, new FastRandom(), false, null));
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

        /// <summary>Verifies an infinite loop exception is caught and given details.</summary>
        [Theory, RandomData]
        public static void Create_InfiniteLoopDetails(Type type, Fake<CreateHint> hint)
        {
            hint.Setup(
                m => m.TryCreate(type, Arg.Any<RandomizerChainer>()),
                Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

            InsufficientExecutionStackException e = Tools.Asserter.Throws<InsufficientExecutionStackException>(
                () => new Randomizer(Tools.Faker, new FastRandom(), false, hint.Dummy).Create(type));

            Tools.Asserter.Is(true, e.Message.Contains(type.Name));
        }
    }
}
