﻿using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.ValuerTool;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.FakerTool.TestSamples;

namespace CreateAndFakeTests.Toolbox.FakerTool;

public static class Fake_T_Tests
{
    [Fact]
    internal static void Fake_GuardsNulls()
    {
        ((IFaked)null).Assert(v => new Fake<object>(v)).Throws<ArgumentNullException>();
        ((Fake)null).Assert(v => new Fake<object>(v)).Throws<ArgumentNullException>();
    }

    [Fact]
    internal static void Setup_GuardsNulls()
    {
        Tools.Faker.Stub<IFakeSample>()
            .Assert(s => s.Setup(null, Behavior.None()))
            .Throws<ArgumentNullException>();
    }

    [Fact]
    internal static void Fake_InterfacesWork()
    {
        FakeTester<IFakeSample>();
    }

    [Fact]
    internal static void Fake_ClassesWork()
    {
        FakeTester<AbstractFakeSample>();
        FakeTester<VirtualFakeSample>();
    }

    [Theory, RandomData]
    internal static void Fake_ScopeBehavior(string name)
    {
        Fake<ScopeSample> fake = Tools.Faker.Mock<ScopeSample>();
        fake.Dummy.Assert(d => d.PublicProp).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.PublicProp = name).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.PublicMethod()).Throws<FakeCallException>();

        fake.Dummy.Assert(d => d.CallProtectProp()).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.SetProtectProp(name)).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.CallProtectGet()).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.SetProtectSet(name)).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.CallProtectMethod()).Throws<FakeCallException>();

        fake.Dummy.Assert(d => d.ProIntProp).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.ProIntProp = name).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.ProIntGet).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.ProIntSet = name).Throws<FakeCallException>();
        fake.Dummy.Assert(d => d.ProIntMethod()).Throws<FakeCallException>();

        fake.Dummy.InternalProp = name;
        fake.Dummy.InternalProp.Assert().Is(name);
        fake.Dummy.InternalMethod().Assert().IsNot(null);

        fake.Dummy.Assert(d => d.InternalGet = name).Throws<FakeCallException>();
        fake.Dummy.InternalGet.Assert().Is(null);
        fake.Dummy.InternalSet = name;
        fake.Dummy.Assert(d => d.InternalSet).Throws<FakeCallException>();
    }

    [Theory, RandomData]
    internal static void Fake_HandlesOut(string data)
    {
        Fake<OutSample> fake = Tools.Faker.Mock<OutSample>();
        fake.Setup(
            d => d.ReturnVoid(out Arg.AnyRef<string>().Var),
            Behavior.Set((OutRef<string> d) => { d.Var = data; }, Times.Once));

        fake.Dummy.ReturnVoid(out string value);

        value.Assert().Is(data);
        fake.VerifyAll(Times.Once);
    }

    [Theory, RandomData]
    internal static void Fake_HandlesOutValue(int plain, int data)
    {
        Fake<OutSample> fake = Tools.Faker.Mock<OutSample>();
        fake.Setup(
            d => d.ReturnValue(out Arg.AnyRef<int>().Var),
            Behavior.Set((OutRef<int> d) =>
            {
                d.Var = data;
                return plain;
            }, Times.Once));

        fake.Dummy.ReturnValue(out int value).Assert().Is(plain);

        value.Assert().Is(data);
        fake.VerifyAll(Times.Once);
    }

    [Theory, RandomData]
    internal static void Fake_HandlesRef(string data, string start)
    {
        Fake<RefSample> fake = Tools.Faker.Mock<RefSample>();
        fake.Setup(
            d => d.ReturnVoid(ref Arg.WhereRef<string>(v => v == start).Var),
            Behavior.Set((OutRef<string> d) => { d.Var = data; }, Times.Once));

        string value = start;
        fake.Dummy.ReturnVoid(ref value);

        value.Assert().Is(data);
        fake.VerifyAll(Times.Once);
    }

    [Theory, RandomData]
    internal static void Fake_GenericsWork(string text, DataSample sample)
    {
        Fake<GenericSample<string>> fake = Tools.Faker.Mock<GenericSample<string>>();

        fake.Setup(
            m => m.Run<DataSample, bool>(text, sample),
            Behavior.Returns(true, Times.Once));
        fake.Setup(
            m => m.Run<DataSample, int>(text, sample),
            Behavior.Returns(5, Times.Once));

        fake.Dummy.Run<DataSample, bool>(text, sample).Assert().Is(true);
        fake.Dummy.Run<DataSample, int>(text, sample).Assert().Is(5);

        fake.VerifyAll(Times.Exactly(2));
        fake.Dummy.Assert(d => d.Run<DataSample, object>(text, sample)).Throws<FakeCallException>();
    }

    [Fact]
    internal static void Setup_InvalidExpressionThrows()
    {
        Tools.Faker.Mock<object>()
            .Assert(f => f.Setup(d => new object(), Behavior.Returns(new object())))
            .Throws<InvalidOperationException>();
    }

    [Theory, RandomData]
    internal static void ConvertArg_ConvertExpression([Stub] IValuer valuer, string[] data)
    {
        valuer.ToFake().Setup(
            m => m.Equals(true, true),
            Behavior.Returns(true));
        valuer.ToFake().Setup(
            m => m.Compare(true, Arg.Any<bool?>()),
            Behavior.Set((object o1, object o2) =>
            {
                return (!o1.Equals(o2))
                    ? Tools.Randomizer.Create<IEnumerable<Difference>>()
                    : [];
            }));

        Asserter tester = new(Tools.Gen, valuer);
        tester.IsNotEmpty(data);
        tester.Assert(t => t.IsNotEmpty(null)).Throws<AssertException>();
        tester.Assert(t => t.IsNotEmpty(Array.Empty<string>())).Throws<AssertException>();
    }

    private static void FakeTester<T>() where T : IFakeSample
    {
        Fake<T> fake = Tools.Faker.Mock<T>(typeof(IClashingFakeSample));

        Behavior<string> hintBehavior = Behavior.Returns("Hint");
        fake.Setup(m => m.Hint, hintBehavior);
        fake.Verify(Times.Never, m => m.Hint);
        fake.Dummy.Hint.Assert().Is("Hint");
        fake.Verify(Times.Once, m => m.Hint);
        fake.Dummy.Hint.Assert().Is("Hint");
        fake.Verify(Times.Exactly(2), m => m.Hint);

        Fake<IClashingFakeSample> fake2 = fake.ToFake<IClashingFakeSample>();
        fake2.SetupSet(m => m.Text, Arg.LambdaAny<string>(), Behavior.None());
        fake2.VerifySet(Times.Never, m => m.Text, Arg.LambdaAny<string>());
        fake2.Dummy.Text = "What";
        fake2.VerifySet(Times.Once, m => m.Text, Arg.LambdaAny<string>());

        fake2.SetupSet(m => m.Text, "Hinter", Behavior.Set((string input) => { }));
        fake2.VerifySet(Times.Never, m => m.Text, "Hinter");
        fake2.Dummy.Text = "Hinter";
        fake2.VerifySet(Times.Once, m => m.Text, "Hinter");
        fake2.VerifySet(Times.Exactly(2), m => m.Text, Arg.LambdaAny<string>());

        fake.Setup(m => m.Read(), Behavior.Set(() => "Test"));
        fake.Verify(Times.Never, m => m.Read());
        fake.Dummy.Read().Assert().Is("Test");
        fake.Verify(Times.Once, m => m.Read());

        fake.Setup(m => m.Calc(), Behavior.Returns(5));
        fake.Verify(Times.Never, m => m.Calc());
        fake.Dummy.Calc().Assert().Is(5);
        fake.Verify(Times.Once, m => m.Calc());

        fake.Setup(m => m.Read("Hey"), Behavior.Returns("Test2"));
        fake.Verify(Times.Never, m => m.Read("Hey"));
        fake.Dummy.Read("Hey").Assert().Is("Test2");
        fake.Verify(Times.Once, m => m.Read("Hey"));

        fake.Setup(m => m.Calc(3), Behavior.Returns(4));
        fake.Dummy.Calc(3).Assert().Is(4);

        fake.Dummy.Read().Assert().Is("Test");
        fake.Dummy.Calc().Assert().Is(5);
        fake.Dummy.Read("Hey").Assert().Is("Test2");

        fake.Setup(m => m.Calc(Arg.Where<int>(i => i > 8)), Behavior.Returns(1));
        fake.Verify(Times.Never, m => m.Calc(Arg.Where<int>(i => i > 8)));
        fake.Dummy.Calc(9);
        fake.Verify(Times.Once, m => m.Calc(Arg.Where<int>(i => i > 8)));

        fake.Setup(m => m.Calc(Arg.Any<int>()), Behavior.Returns(7));
        fake.Dummy.Calc(0).Assert().Is(7);

        fake.Setup(m => m.Read(Arg.Any<string>()), Behavior.Returns("Wow!"));
        fake.Dummy.Read("Okay?").Assert().Is("Wow!");

        fake.Setup(m => m.Combo(2, "Finally"), Behavior.Set((int num, string text) => { }));
        fake.Verify(Times.Never, m => m.Combo(2, "Finally"));
        fake.Dummy.Combo(2, "Finally");
        fake.Verify(Times.Once, m => m.Combo(2, "Finally"));

        hintBehavior.Calls.Assert().Is(2);
    }
}
