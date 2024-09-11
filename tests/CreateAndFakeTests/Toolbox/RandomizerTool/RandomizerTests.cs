﻿using System.Collections;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.RandomizerTool;

public static class RandomizerTests
{
    [Fact]
    internal static void Randomizer_GuardsNulls()
    {
        Tools.Asserter.Throws<ArgumentNullException>(() => new Randomizer(null, Tools.Gen, Limiter.Few));
        Tools.Asserter.Throws<ArgumentNullException>(() => new Randomizer(Tools.Faker, null, Limiter.Few));
        Tools.Asserter.Throws<ArgumentNullException>(() => new Randomizer(Tools.Faker, Tools.Gen, null));

        Tools.Tester.PreventsNullRefException(Tools.Randomizer);
    }

    [Fact]
    internal static void New_NullHintsValid()
    {
        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, true, null).Assert().IsNot(null);
        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, null).Assert().IsNot(null);
    }

    [Fact]
    internal static void Create_NoRulesThrows()
    {
        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false)
            .Assert(r => r.Create<object>())
            .Throws<NotSupportedException>();
    }

    [Theory, RandomData]
    internal static void CreateSized_NoRulesThrows(int size)
    {
        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false)
            .Assert(r => r.CreateSized<IEnumerable>(size))
            .Throws<NotSupportedException>();
    }

    [Theory, RandomData]
    internal static void Create_MissingMatchThrows([Stub] CreateHint hint, string data)
    {
        hint.ToFake().ThrowByDefault = true;
        hint.ToFake().Setup(
            m => m.TryCreate(data.GetType(), Arg.Any<RandomizerChainer>()),
            Behavior.Returns((false, (object)null), Times.Once));

        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, hint)
            .Assert(r => r.Create<string>())
            .Throws<NotSupportedException>();

        hint.VerifyAllCalls(Times.Once);
    }

    [Theory, RandomData]
    internal static void CreateSized_MissingMatchThrows([Fake] CreateCollectionHint hint, int size, string data)
    {
        hint.ToFake().Setup(
            m => m.TryCreate(data.GetType(), size, Arg.Any<RandomizerChainer>()),
            Behavior.Returns((false, (object)null), Times.Once));

        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, hint)
            .Assert(r => r.CreateSized<string>(size))
            .Throws<NotSupportedException>();

        hint.VerifyAllCalls(Times.Once);
    }

    [Theory, RandomData]
    internal static void Create_ValidHintWorks([Stub] CreateHint hint, string data)
    {
        hint.ToFake().ThrowByDefault = true;
        hint.ToFake().Setup(
            m => m.TryCreate(data.GetType(), Arg.Any<RandomizerChainer>()),
            Behavior.Returns((true, (object)data), Times.Once));

        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, hint)
            .Create<string>()
            .Assert()
            .Is(data);

        hint.VerifyAllCalls(Times.Once);
    }

    [Theory, RandomData]
    internal static void CreateSized_ValidHintWorks([Stub] CreateCollectionHint hint, string[] data)
    {
        hint.ToFake().ThrowByDefault = true;
        hint.ToFake().Setup(
            m => m.TryCreate(data.GetType(), 1, Arg.Any<RandomizerChainer>()),
            Behavior.Returns((true, (object)data), Times.Once));

        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, hint)
            .CreateSized<string[]>(1)
            .Assert()
            .Is(data);

        hint.VerifyAllCalls(Times.Once);
    }

    [Theory, RandomData]
    internal static void Create_InfiniteLoopDetails(Type type, [Fake] CreateHint hint)
    {
        hint.ToFake().Setup(
            m => m.TryCreate(type, Arg.Any<RandomizerChainer>()),
            Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, hint)
            .Assert(r => r.Create(type))
            .Throws<InsufficientExecutionStackException>().Message
            .Assert()
            .Contains(type.Name);
    }

    [Theory, RandomData]
    internal static void CreateSized_InfiniteLoopDetails(Type type, [Fake] CreateCollectionHint hint, int size)
    {
        hint.ToFake().Setup(
            m => m.TryCreate(type, size, Arg.Any<RandomizerChainer>()),
            Behavior.Throw<InsufficientExecutionStackException>(Times.Once));

        new Randomizer(Tools.Faker, new FastRandom(), Limiter.Dozen, false, hint)
            .Assert(r => r.CreateSized(type, size))
            .Throws<InsufficientExecutionStackException>().Message
            .Assert()
            .Contains(type.Name);
    }

    [Fact]
    internal static void Create_ConditionMatchReturned()
    {
        Tools.Randomizer.Create<int>(v => v < 0).Assert().LessThan(0);
    }

    [Fact]
    internal static void Create_ConditionTimesOut()
    {
        Tools.Randomizer
            .Assert(r => r.Create<DateTime>(d => d < DateTime.MinValue))
            .Throws<TimeoutException>();
    }

    [Theory, RandomData]
    internal static void Inject_SingleFakeInjected(Fake<DataSample> fake, InjectSample holder)
    {
        holder.Data.Assert().ReferenceEqual(fake.Dummy);
        holder.Data2.Assert().ReferenceNotEqual(fake.Dummy);
    }

    [Theory, RandomData]
    internal static void Inject_DoubleFakeInjected(Fake<DataSample> fake, [Fake] DataSample fake2, InjectSample holder)
    {
        holder.Data2.Assert().ReferenceEqual(fake.Dummy);
        holder.Data.Assert().ReferenceEqual(fake2);
    }

    [Theory, RandomData]
    internal static void Inject_InterfaceFakesInjected(
        Fake<IOnlyMockSample> fake, Fake<IOnlyMockSample> fake2, InjectMockSample holder)
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
        Tools.Randomizer
            .CreateFor(typeof(InjectMockSample).GetConstructors().Single(), fake, fake2).Args.ToArray()
            .Assert()
            .Is(new object[] { fake.Dummy, fake2.Dummy });
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
        injected.Dummy.Assert().IsNot(sample);
        injected.Fakes.Contains(inner1).Assert().Is(false);
        injected.Fakes.Contains(inner2).Assert().Is(false);

        injected.Dummy.TestIfMockedSeparately();
    }
}
