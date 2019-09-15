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
        private readonly Asserter _testInstance;

        /// <summary>Faked valuer to test with.</summary>
        private readonly Fake<IValuer> _fakeValuer;

        /// <summary>Sets up the test instance.</summary>
        public AsserterTests()
        {
            _fakeValuer = Tools.Faker.Mock<IValuer>();
            _testInstance = new Asserter(_fakeValuer.Dummy);
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

            _testInstance.CheckAll(
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
                () => _testInstance.CheckAll(
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
                () => _testInstance.CheckAll(
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
            _testInstance.Throws<InvalidOperationException>((Action)
                (() => throw new InvalidOperationException()));
        }

        /// <summary>Verifies failure when actions throw the wrong exception.</summary>
        [Fact]
        public void Throws_ActionTypeMismatch()
        {
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<ArgumentException>(
                    (Action)(() => throw new NotImplementedException())));
        }

        /// <summary>Verifies failure when actions don't throw an exception.</summary>
        [Fact]
        public void Throws_ActionNoThrowFail()
        {
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<InvalidOperationException>(() => { }));
        }

        /// <summary>Verifies failure when null actions don't throw an exception.</summary>
        [Fact]
        public void Throws_ActionNullCase()
        {
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<InvalidOperationException>((Action)null));
        }

        /// <summary>Verifies success when funcs throw.</summary>
        [Fact]
        public void Throws_FuncThrowsSuccess()
        {
            _testInstance.Throws<InvalidOperationException>(
                () => throw new InvalidOperationException());
        }

        /// <summary>Verifies failure when funcs throw the wrong exception.</summary>
        [Fact]
        public void Throws_FuncTypeMismatch()
        {
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<ArgumentException>(
                    () => throw new NotImplementedException()));
        }

        /// <summary>Verifies failure when funcs don't throw an exception.</summary>
        [Fact]
        public void Throws_FuncNoThrowFail()
        {
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<InvalidOperationException>(() => true));
        }

        /// <summary>Verifies failure when null funcs don't throw an exception.</summary>
        [Fact]
        public void Throws_FuncNullCase()
        {
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<InvalidOperationException>(null));
        }

        /// <summary>Verifies disposable behavior upon failure case.</summary>
        [Theory, RandomData]
        public void Throws_Disposes(Fake<IDisposable> disposable)
        {
            disposable.Setup(m => m.Dispose(), Behavior.None(Times.Once));

            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<Exception>(() => disposable.Dummy));

            disposable.VerifyAll(Times.Once);
        }

        /// <summary>Verifies aggregate exception wrapping is ignored.</summary>
        [Fact]
        public void Throws_AggregateUnwraps()
        {
            AggregateException ex = new AggregateException(new InvalidOperationException());

            _testInstance.Throws<InvalidOperationException>(() => throw ex);
        }

        /// <summary>Verifies aggregate exception still fails if internals aren't expected.</summary>
        [Fact]
        public void Throws_AggregateExtraInternal()
        {
            AggregateException ex = new AggregateException(
                new InvalidOperationException(), new InvalidCastException());

            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<InvalidOperationException>(() => throw ex));
        }

        /// <summary>Verifies aggregate exception still fails if internals aren't expected.</summary>
        [Fact]
        public void Throws_AggregateWrongInternals()
        {
            AggregateException ex = new AggregateException(new InvalidOperationException());

            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.Throws<InvalidCastException>(() => throw ex));
        }

        /// <summary>Verifies the expected cases.</summary>
        [Fact]
        public void IsEmpty_Works()
        {
            _testInstance.IsEmpty(Array.Empty<string>());

            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.IsEmpty(null));
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.IsEmpty(Tools.Randomizer.Create<string[]>()));
        }

        /// <summary>Verifies the expected cases.</summary>
        [Fact]
        public void IsNotEmpty_Works()
        {
            _fakeValuer.Setup(
                m => m.Equals(true, true),
                Behavior.Returns(true));
            _fakeValuer.Setup(
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

            _testInstance.IsNotEmpty(Tools.Randomizer.Create<string[]>());

            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.IsNotEmpty(null));
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.IsNotEmpty(Array.Empty<string>()));
        }

        /// <summary>Verifies the expected cases.</summary>
        [Theory, RandomData]
        public void HasCount_Works(IEnumerable<string> data)
        {
            _testInstance.HasCount(data.Count(), data);

            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.HasCount(data.Count(), null));
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.HasCount(data.Count() - 1, data));
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.HasCount(data.Count() + 1, data));
        }

        /// <summary>Verifies equality comparison is not used.</summary>
        [Theory, RandomData]
        public void ReferenceEqual_NotByValue(Fake<object> fake)
        {
            fake.Setup(
                m => m.Equals(Arg.Any<object>()),
                Behavior.Returns(true, Times.Never));

            _testInstance.ReferenceEqual(fake.Dummy, fake.Dummy);
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.ReferenceEqual(fake.Dummy, Tools.Duplicator.Copy(fake.Dummy)));

            fake.VerifyAll(Times.Never);
        }

        /// <summary>Verifies equality comparison is not used.</summary>
        [Theory, RandomData]
        public void ReferenceNotEqual_NotByValue(Fake<object> fake)
        {
            fake.Setup(
                m => m.Equals(Arg.Any<object>()),
                Behavior.Returns(false, Times.Never));

            _testInstance.ReferenceNotEqual(fake.Dummy, Tools.Duplicator.Copy(fake.Dummy));
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.ReferenceNotEqual(fake.Dummy, fake.Dummy));

            fake.VerifyAll(Times.Never);
        }

        /// <summary>Verifies valid when no differences are found.</summary>
        [Fact]
        public void ValuesEqual_EqualValid()
        {
            _fakeValuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));

            _testInstance.ValuesEqual(new object(), new object());

            _fakeValuer.VerifyAll(Times.Once);
        }

        /// <summary>Verifies invalid when differences are found.</summary>
        [Fact]
        public void ValuesEqual_UnequalInvalid()
        {
            _fakeValuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>(), Times.Once));
            _fakeValuer.Setup(
                m => m.Compare(null, Arg.Any<object>()),
                Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>(), Times.Once));

            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.ValuesEqual(new object(), new object()));
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.ValuesEqual(null, new object()));

            _fakeValuer.VerifyAll(Times.Exactly(2));
        }

        /// <summary>Verifies invalid when no differences are found.</summary>
        [Fact]
        public void ValuesNotEqual_EqualInvalid()
        {
            _fakeValuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));
            _fakeValuer.Setup(
                m => m.Compare(null, Arg.Any<object>()),
                Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));

            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.ValuesNotEqual(new object(), new object()));
            Tools.Asserter.Throws<AssertException>(
                () => _testInstance.ValuesNotEqual(null, new object()));

            _fakeValuer.VerifyAll(Times.Exactly(2));
        }

        /// <summary>Verifies invalid when differences are found.</summary>
        [Fact]
        public void ValuesNotEqual_UnequalValid()
        {
            _fakeValuer.Setup(
                m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
                Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>(), Times.Once));

            _testInstance.ValuesNotEqual(new object(), new object());

            _fakeValuer.VerifyAll(Times.Once);
        }
    }
}
