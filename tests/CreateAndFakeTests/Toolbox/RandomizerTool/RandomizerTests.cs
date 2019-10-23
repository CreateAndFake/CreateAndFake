using System;
using System.Linq;
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
            Tools.Tester.PreventsNullRefException<Randomizer>(Tools.Faker, Tools.Gen, Limiter.Few);
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
        internal static void Inject_SingleFakeInjected(Fake<DataSample> fake, InjectSample holder)
        {
            Tools.Asserter.ReferenceEqual(fake.Dummy, holder.Data);
            Tools.Asserter.ReferenceNotEqual(fake.Dummy, holder.Data2);
        }

        [Theory, RandomData]
        internal static void Inject_DoubleFakeInjected(Fake<DataSample> fake, Fake<DataSample> fake2, InjectSample holder)
        {
            Tools.Asserter.ReferenceEqual(fake.Dummy, holder.Data2);
            Tools.Asserter.ReferenceEqual(fake2.Dummy, holder.Data);
        }

        [Theory, RandomData]
        internal static void Inject_InterfaceFakesInjected(Fake<IOnlyMockSample> fake,
            Fake<IOnlyMockSample> fake2, InjectMockSample holder)
        {
            fake.Verify(Times.Never, f => f.FailIfNotMocked());
            fake2.Verify(Times.Never, f => f.FailIfNotMocked());
            holder.TestIfMockedSeparately();
            fake.Verify(Times.Once, f => f.FailIfNotMocked());
            fake2.Verify(Times.Once, f => f.FailIfNotMocked());
        }

        [Theory, RandomData]
        internal static void CreateFor_InterfaceFakesInjected(Fake<IOnlyMockSample> fake, Fake<IOnlyMockSample> fake2)
        {
            Tools.Asserter.Is(new object[] { fake.Dummy, fake2.Dummy }, Tools.Randomizer.CreateFor(
                typeof(InjectMockSample).GetConstructors().Single(), fake, fake2));
        }

        [Theory, RandomData]
        internal static void CreateFor_InjectedWorks(Injected<InjectMockSample> injected)
        {
            injected.Dummy.TestIfMockedSeparately();
        }

        [Theory, RandomData]
        internal static void CreateFor_InjectedNotManuallyInjected(Fake<IOnlyMockSample> inner1,
            Fake<IOnlyMockSample> inner2, InjectMockSample sample, Injected<InjectMockSample> injected)
        {
            Tools.Asserter.IsNot(sample, injected.Dummy);
            Tools.Asserter.Is(false, injected.Fakes.Contains(inner1));
            Tools.Asserter.Is(false, injected.Fakes.Contains(inner2));

            injected.Dummy.TestIfMockedSeparately();
        }
    }
}
