using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool
{
    /// <summary>Verifies behavior.</summary>
    public sealed class AsserterTests
    {
        /// <summary>Instance to test with.</summary>
        private readonly Asserter m_TestInstance;

        /// <summary>Faked valuer to test with.</summary>
        private readonly Fake<IValuer> m_FakeValuer;

        /// <summary>Sets up the test instance.</summary>
        public AsserterTests()
        {
            m_FakeValuer = Tools.Faker.Mock<IValuer>();
            m_TestInstance = new Asserter(m_FakeValuer.Dummy);
        }

        /// <summary>Verifies openness for custom individual behavior by inheritance.</summary>
        [Fact]
        public static void Asserter_AllMethodsVirtual()
        {
            Tools.Asserter.IsEmpty(typeof(Asserter)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsVirtual)
                .Select(m => m.Name)
                .Where(n => n != "Is" && n != "IsNot"));
        }

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void Asserter_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<Asserter>();
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void Asserter_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<Asserter>();
        }

        /// <summary>Verifies each case is run.</summary>
        [Fact]
        public void CheckAll_ValidRunsAll()
        {
            bool ran1 = false;
            bool ran2 = false;

            m_TestInstance.CheckAll(
                () => ran1 = true,
                () => ran2 = true);

            Tools.Asserter.Is(true, ran1);
            Tools.Asserter.Is(true, ran2);
        }

        /// <summary>Verifies only one error is thrown.</summary>
        [Theory, RandomData]
        public void CheckAll_SingleErrorThrows(Exception error)
        {
            bool ran2 = false;

            AggregateException result = Tools.Asserter.Throws<AggregateException>(
                () => m_TestInstance.CheckAll(
                    () => throw error,
                    () => ran2 = true));

            Tools.Asserter.Is(true, ran2);
            Tools.Asserter.Is(result.InnerExceptions.ToArray(), new[] { error });
        }

        /// <summary>Verifies each case is run.</summary>
        [Theory, RandomData]
        public void CheckAll_ErrorRunsAll(Exception error1, Exception error2)
        {
            AggregateException result = Tools.Asserter.Throws<AggregateException>(
                () => m_TestInstance.CheckAll(
                    () => throw error1,
                    () => throw error2));

            Tools.Asserter.Is(result.InnerExceptions.ToArray(), new[] { error1, error2 });
        }

        /// <summary>Verifies fail will throw.</summary>
        [Fact]
        public static void Fail_Throws()
        {
            Tools.Asserter.Throws<AssertException>(() => Tools.Asserter.Fail());
        }

        /// <summary>Verifies fail will throw.</summary>
        [Theory, RandomData]
        public static void Fail_ThrowsWithException(Exception error)
        {
            Tools.Asserter.Is(error, Tools.Asserter.Throws<AssertException>(
                () => Tools.Asserter.Fail(error)).InnerException);
        }

        /// <summary>Verifies success when actions throw.</summary>
        [Fact]
        public void Throws_ActionThrowsSuccess()
        {
            m_TestInstance.Throws<InvalidOperationException>((Action)
                (() => throw new InvalidOperationException()));
        }

        /// <summary>Verifies failure when actions throw the wrong exception.</summary>
        [Fact]
        public void Throws_ActionTypeMismatch()
        {
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<ArgumentException>(
                    (Action)(() => throw new NotImplementedException())));
        }

        /// <summary>Verifies failure when actions don't throw an exception.</summary>
        [Fact]
        public void Throws_ActionNoThrowFail()
        {
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<InvalidOperationException>(() => { }));
        }

        /// <summary>Verifies failure when null actions don't throw an exception.</summary>
        [Fact]
        public void Throws_ActionNullCase()
        {
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<InvalidOperationException>((Action)null));
        }

        /// <summary>Verifies success when funcs throw.</summary>
        [Fact]
        public void Throws_FuncThrowsSuccess()
        {
            m_TestInstance.Throws<InvalidOperationException>(
                () => throw new InvalidOperationException());
        }

        /// <summary>Verifies failure when funcs throw the wrong exception.</summary>
        [Fact]
        public void Throws_FuncTypeMismatch()
        {
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<ArgumentException>(
                    () => throw new NotImplementedException()));
        }

        /// <summary>Verifies failure when funcs don't throw an exception.</summary>
        [Fact]
        public void Throws_FuncNoThrowFail()
        {
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<InvalidOperationException>(() => true));
        }

        /// <summary>Verifies failure when null funcs don't throw an exception.</summary>
        [Fact]
        public void Throws_FuncNullCase()
        {
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<InvalidOperationException>(null));
        }

        /// <summary>Verifies disposable behavior upon failure case.</summary>
        [Theory, RandomData]
        public void Throws_Disposes(Fake<IDisposable> disposable)
        {
            disposable.Setup(m => m.Dispose(), Behavior.None(Times.Once));

            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<Exception>(() => disposable.Dummy));

            disposable.VerifyAll(Times.Once);
        }

        /// <summary>Verifies aggregate exception wrapping is ignored.</summary>
        [Fact]
        public void Throws_AggregateUnwraps()
        {
            AggregateException ex = new AggregateException(new InvalidOperationException());

            m_TestInstance.Throws<InvalidOperationException>(() => throw ex);
        }

        /// <summary>Verifies aggregate exception still fails if internals aren't expected.</summary>
        [Fact]
        public void Throws_AggregateExtraInternal()
        {
            AggregateException ex = new AggregateException(
                new InvalidOperationException(), new InvalidCastException());

            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<InvalidOperationException>(() => throw ex));
        }

        /// <summary>Verifies aggregate exception still fails if internals aren't expected.</summary>
        [Fact]
        public void Throws_AggregateWrongInternals()
        {
            AggregateException ex = new AggregateException(new InvalidOperationException());

            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.Throws<InvalidCastException>(() => throw ex));
        }

        /// <summary>Verifies the expected cases.</summary>
        [Fact]
        public void IsEmpty_Works()
        {
            m_TestInstance.IsEmpty(Array.Empty<string>());

            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.IsEmpty(null));
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.IsEmpty(Tools.Randomizer.Create<string[]>()));
        }

        /// <summary>Verifies the expected cases.</summary>
        [Fact]
        public void IsNotEmpty_Works()
        {
            m_FakeValuer.Setup(
                m => m.Equals(true, true),
                Behavior.Returns(true));
            m_FakeValuer.Setup(
                m => m.Compare(true, Arg.Any<bool?>()),
                Behavior.Set((object o1, object o2) =>
                {
                    if (!o1.Equals(o2))
                    {
                        return Tools.Randomizer.Create<IEnumerable<Difference>>();
                    }
                    else
                    {
                        return Enumerable.Empty<Difference>();
                    }
                }));

            m_TestInstance.IsNotEmpty(Tools.Randomizer.Create<string[]>());

            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.IsNotEmpty(null));
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.IsNotEmpty(Array.Empty<string>()));
        }

        /// <summary>Verifies the expected cases.</summary>
        [Theory, RandomData]
        public void HasCount_Works(IEnumerable<string> data)
        {
            m_TestInstance.HasCount(data.Count(), data);

            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.HasCount(data.Count(), null));
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.HasCount(data.Count() - 1, data));
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.HasCount(data.Count() + 1, data));
        }

        /// <summary>Verifies equality comparison is not used.</summary>
        [Theory, RandomData]
        public void ReferenceEqual_NotByValue(Fake<object> fake)
        {
            fake.Setup(
                m => m.Equals(Arg.Any<object>()),
                Behavior.Returns(true, Times.Never));

            m_TestInstance.ReferenceEqual(fake.Dummy, fake.Dummy);
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.ReferenceEqual(fake.Dummy, Tools.Duplicator.Copy(fake.Dummy)));

            fake.VerifyAll(Times.Never);
        }

        /// <summary>Verifies equality comparison is not used.</summary>
        [Theory, RandomData]
        public void ReferenceNotEqual_NotByValue(Fake<object> fake)
        {
            fake.Setup(
                m => m.Equals(Arg.Any<object>()),
                Behavior.Returns(false, Times.Never));

            m_TestInstance.ReferenceNotEqual(fake.Dummy, Tools.Duplicator.Copy(fake.Dummy));
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.ReferenceNotEqual(fake.Dummy, fake.Dummy));

            fake.VerifyAll(Times.Never);
        }

        /// <summary>Verifies valid when no differences are found.</summary>
        [Fact]
        public void ValuesEqual_EqualValid()
        {
            m_FakeValuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));

            m_TestInstance.ValuesEqual(new object(), new object());

            m_FakeValuer.VerifyAll(Times.Once);
        }

        /// <summary>Verifies invalid when differences are found.</summary>
        [Fact]
        public void ValuesEqual_UnequalInvalid()
        {
            m_FakeValuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>(), Times.Once));
            m_FakeValuer.Setup(
                m => m.Compare(null, Arg.Any<object>()),
                Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>(), Times.Once));

            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.ValuesEqual(new object(), new object()));
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.ValuesEqual(null, new object()));

            m_FakeValuer.VerifyAll(Times.Exactly(2));
        }

        /// <summary>Verifies invalid when no differences are found.</summary>
        [Fact]
        public void ValuesNotEqual_EqualInvalid()
        {
            m_FakeValuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));
            m_FakeValuer.Setup(
                m => m.Compare(null, Arg.Any<object>()),
                Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));

            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.ValuesNotEqual(new object(), new object()));
            Tools.Asserter.Throws<AssertException>(
                () => m_TestInstance.ValuesNotEqual(null, new object()));

            m_FakeValuer.VerifyAll(Times.Exactly(2));
        }

        /// <summary>Verifies invalid when differences are found.</summary>
        [Fact]
        public void ValuesNotEqual_UnequalValid()
        {
            m_FakeValuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>(), Times.Once));

            m_TestInstance.ValuesNotEqual(new object(), new object());

            m_FakeValuer.VerifyAll(Times.Once);
        }
    }
}
