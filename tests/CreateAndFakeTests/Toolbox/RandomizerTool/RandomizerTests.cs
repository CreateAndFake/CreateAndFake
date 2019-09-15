using System;
using CreateAndFake;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class RandomizerTests
    {
        [Fact]
        internal static void Randomizer_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(Tools.Randomizer);
        }

        [Fact]
        internal static void Randomizer_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(Tools.Randomizer);
        }

        [Fact]
        internal static void New_NullHintsValid()
        {
            Tools.Asserter.IsNot(null, new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, true, null));
            Tools.Asserter.IsNot(null, new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, null));
        }

        [Fact]
        internal static void Create_NoRulesThrows()
        {
            Tools.Asserter.Throws<NotSupportedException>(
                () => new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false).Create<object>());
        }

        [Fact]
        internal static void Create_MissingMatchThrows()
        {
            string data = "Result";

            Fake<CreateHint> hint = Tools.Faker.Mock<CreateHint>();
            hint.Setup(
                m => m.TryCreate(data.GetType(), Arg.Any<RandomizerChainer>()),
                Behavior.Returns((false, (object)null), Times.Once));

            Tools.Asserter.Throws<NotSupportedException>(
                () => new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, hint.Dummy).Create<string>());

            hint.VerifyAll(Times.Once);
        }

        [Fact]
        internal static void Create_ValidHintWorks()
        {
            string data = "Result";

            Fake<CreateHint> hint = Tools.Faker.Mock<CreateHint>();
            hint.Setup(
                m => m.TryCreate(data.GetType(), Arg.Any<RandomizerChainer>()),
                Behavior.Returns((true, (object)data), Times.Once));

            Tools.Asserter.Is(data, new Randomizer(Tools.Faker, new FastRandom(),
                Limiter.Dozen, false, hint.Dummy).Create<string>());

            hint.VerifyAll(Times.Once);
        }

        [Theory, RandomData]
        internal static void Create_InfiniteLoopDetails(Type type, Fake<CreateHint> hint)
        {
            hint.Setup(
                m => m.TryCreate(type, Arg.Any<RandomizerChainer>()),
                Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

            InsufficientExecutionStackException e = Tools.Asserter.Throws<InsufficientExecutionStackException>(
                () => new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, hint.Dummy).Create(type));

            Tools.Asserter.Is(true, e.Message.Contains(type.Name));
        }

        [Fact]
        internal static void Create_ConditionMatchReturned()
        {
            int result = Tools.Randomizer.Create<int>(v => v < 0);
            Tools.Asserter.Is(true, result < 0);
        }

        [Fact]
        internal static void Create_ConditionTimesOut()
        {
            Tools.Asserter.Throws<TimeoutException>(
                () => Tools.Randomizer.Create<DateTime>(d => d < DateTime.MinValue));
        }

        [Theory, RandomData]
        internal static void Inject_FakesAreInjected(Fake<DataSample> fake, InjectSample holder)
        {
            Tools.Asserter.ReferenceEqual(fake.Dummy, holder.Data);
        }
    }
}
