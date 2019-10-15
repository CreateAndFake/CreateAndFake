using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.FakerTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class Fake_T_Tests
    {
        [Fact]
        internal static void Fake_GuardsNulls()
        {
            Tools.Asserter.Throws<ArgumentNullException>(() => new Fake<object>(null));
        }

        [Fact]
        internal static void Setup_GuardsNulls()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => Tools.Faker.Stub<IFakeSample>().Setup(null, Behavior.None()));
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

        [Fact]
        internal static void Fake_ScopeBehavior()
        {
            Fake<ScopeSample> fake = Tools.Faker.Mock<ScopeSample>();
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.PublicProp);
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.PublicProp = "Value");
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.PublicMethod());

            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.CallProtectProp());
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.SetProtectProp("Value"));
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.CallProtectGet());
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.SetProtectSet("Value"));
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.CallProtectMethod());

            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.ProIntProp);
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.ProIntProp = "Value");
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.ProIntGet);
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.ProIntSet = "Value");
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.ProIntMethod());

            string data = Tools.Randomizer.Create<string>();
            fake.Dummy.InternalProp = data;
            Tools.Asserter.Is(data, fake.Dummy.InternalProp);
            Tools.Asserter.IsNot(null, fake.Dummy.InternalMethod());

            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.InternalGet = data);
            Tools.Asserter.Is(null, fake.Dummy.InternalGet);
            fake.Dummy.InternalSet = data;
            Tools.Asserter.Throws<FakeCallException>(() => fake.Dummy.InternalSet);
        }

        [Theory, RandomData]
        internal static void Fake_HandlesOut(string data)
        {
            Fake<OutSample> fake = Tools.Faker.Mock<OutSample>();
            fake.Setup(
                d => d.ReturnVoid(out Arg.AnyRef<string>().Var),
                Behavior.Set((OutRef<string> d) => { d.Var = data; }, Times.Once));

            fake.Dummy.ReturnVoid(out string value);

            Tools.Asserter.Is(data, value);
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

            Tools.Asserter.Is(data, value);
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

            Tools.Asserter.Is(true, fake.Dummy.Run<DataSample, bool>(text, sample));
            Tools.Asserter.Is(5, fake.Dummy.Run<DataSample, int>(text, sample));

            fake.VerifyAll(Times.Exactly(2));

            Tools.Asserter.Throws<FakeCallException>(
                () => fake.Dummy.Run<DataSample, object>(text, sample));
        }

        [Fact]
        internal static void Setup_InvalidExpressionThrows()
        {
            Fake<object> fake = Tools.Faker.Mock<object>();

            Tools.Asserter.Throws<InvalidOperationException>(
                () => fake.Setup(d => new object(), Behavior.Returns(new object())));
        }

        /// <summary>Handles the testing of fake samples.</summary>
        /// <typeparam name="T">Type of sample to test.</typeparam>
        private static void FakeTester<T>() where T : IFakeSample
        {
            Fake<T> fake = Tools.Faker.Mock<T>(typeof(IClashingFakeSample));

            Behavior<string> hintBehavior = Behavior.Returns("Hint");
            fake.Setup(m => m.Hint, hintBehavior);
            fake.Verify(Times.Never, m => m.Hint);
            Tools.Asserter.Is("Hint", fake.Dummy.Hint);
            fake.Verify(Times.Once, m => m.Hint);
            Tools.Asserter.Is("Hint", fake.Dummy.Hint);
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
            Tools.Asserter.Is("Test", fake.Dummy.Read());
            fake.Verify(Times.Once, m => m.Read());

            fake.Setup(m => m.Calc(), Behavior.Returns(5));
            fake.Verify(Times.Never, m => m.Calc());
            Tools.Asserter.Is(5, fake.Dummy.Calc());
            fake.Verify(Times.Once, m => m.Calc());

            fake.Setup(m => m.Read("Hey"), Behavior.Returns("Test2"));
            fake.Verify(Times.Never, m => m.Read("Hey"));
            Tools.Asserter.Is("Test2", fake.Dummy.Read("Hey"));
            fake.Verify(Times.Once, m => m.Read("Hey"));

            fake.Setup(m => m.Calc(3), Behavior.Returns(4));
            Tools.Asserter.Is(4, fake.Dummy.Calc(3));

            Tools.Asserter.Is("Test", fake.Dummy.Read());
            Tools.Asserter.Is(5, fake.Dummy.Calc());
            Tools.Asserter.Is("Test2", fake.Dummy.Read("Hey"));

            fake.Setup(m => m.Calc(Arg.Where<int>(i => i > 8)), Behavior.Returns(1));
            fake.Verify(Times.Never, m => m.Calc(Arg.Where<int>(i => i > 8)));
            fake.Dummy.Calc(9);
            fake.Verify(Times.Once, m => m.Calc(Arg.Where<int>(i => i > 8)));

            fake.Setup(m => m.Calc(Arg.Any<int>()), Behavior.Returns(7));
            Tools.Asserter.Is(7, fake.Dummy.Calc(0));

            fake.Setup(m => m.Read(Arg.Any<string>()), Behavior.Returns("Wow!"));
            Tools.Asserter.Is("Wow!", fake.Dummy.Read("Okay?"));

            fake.Setup(m => m.Combo(2, "Finally"),
                Behavior.Set((int num, string text) => { }));
            fake.Verify(Times.Never, m => m.Combo(2, "Finally"));
            fake.Dummy.Combo(2, "Finally");
            fake.Verify(Times.Once, m => m.Combo(2, "Finally"));

            Tools.Asserter.Is(2, hintBehavior.Calls);
        }
    }
}
