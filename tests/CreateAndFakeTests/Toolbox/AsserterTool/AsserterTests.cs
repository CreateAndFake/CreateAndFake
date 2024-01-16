using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool;

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
        _testInstance = new Asserter(Tools.Gen, _fakeValuer.Dummy);
    }

    [Fact]
    internal static void Asserter_AllMethodsVirtual()
    {
        Tools.Asserter.IsEmpty(typeof(Asserter)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsVirtual)
            .Select(m => m.Name)
            .Where(n => n is not "Is" and not "IsNot"));
    }

    [Fact]
    internal static void Asserter_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<Asserter>();
    }

    [Fact]
    internal static void Asserter_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<Asserter>();
    }

    [Fact]
    internal void CheckAll_RunsEachValidCase()
    {
        bool ran1 = false;
        bool ran2 = false;

        _testInstance.CheckAll(
            () => ran1 = true,
            () => ran2 = true);

        Tools.Asserter.Is(true, ran1);
        Tools.Asserter.Is(true, ran2);
    }

    [Theory, RandomData]
    internal void CheckAll_SingleErrorThrows(Exception error)
    {
        bool ran2 = false;

        AggregateException result = Tools.Asserter.Throws<AggregateException>(
            () => _testInstance.CheckAll(
                () => throw error,
                () => ran2 = true));

        Tools.Asserter.Is(true, ran2);
        Tools.Asserter.Is(result.InnerExceptions.ToArray(), new[] { error });
    }

    [Theory, RandomData]
    internal void CheckAll_RunsEachErrorCase(Exception error1, Exception error2)
    {
        AggregateException result = Tools.Asserter.Throws<AggregateException>(
            () => _testInstance.CheckAll(
                () => throw error1,
                () => throw error2));

        Tools.Asserter.Is(result.InnerExceptions.ToArray(), new[] { error1, error2 });
    }

    [Fact]
    internal static void Fail_Throws()
    {
        Tools.Asserter.Throws<AssertException>(() => Tools.Asserter.Fail());
    }

    [Theory, RandomData]
    internal static void Fail_ThrowsWithException(Exception error)
    {
        Tools.Asserter.Is(error, Tools.Asserter.Throws<AssertException>(
            () => Tools.Asserter.Fail(error)).InnerException);
    }

    [Fact]
    internal void Throws_ActionThrowsSuccess()
    {
        _testInstance.Throws<InvalidOperationException>((Action)
            (() => throw new InvalidOperationException()));
    }

    [Fact]
    internal void Throws_ActionTypeMismatch()
    {
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<ArgumentException>(
                (Action)(() => throw new NotImplementedException())));
    }

    [Fact]
    internal void Throws_ActionNoThrowFail()
    {
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<InvalidOperationException>(() => { }));
    }

    [Fact]
    internal void Throws_ActionNullCase()
    {
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<InvalidOperationException>((Action)null));
    }

    [Fact]
    internal void Throws_FuncThrowsSuccess()
    {
        _testInstance.Throws<InvalidOperationException>(
            () => throw new InvalidOperationException());
    }

    [Fact]
    internal void Throws_FuncTypeMismatch()
    {
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<ArgumentException>(
                () => throw new NotImplementedException()));
    }

    [Fact]
    internal void Throws_FuncNoThrowFail()
    {
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<InvalidOperationException>(() => true));
    }

    [Fact]
    internal void Throws_FuncNullCase()
    {
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<InvalidOperationException>(null));
    }

    [Theory, RandomData]
    internal void Throws_Disposes(Fake<IDisposable> disposable)
    {
        disposable.Setup(m => m.Dispose(), Behavior.None(Times.Once));

        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<Exception>(() => disposable.Dummy));

        disposable.VerifyAll(Times.Once);
    }

    [Fact]
    internal void Throws_AggregateUnwraps()
    {
        AggregateException ex = new(new InvalidOperationException());

        _testInstance.Throws<InvalidOperationException>(() => throw ex);
    }

    [Fact]
    internal void Throws_AggregateExtraInternal()
    {
        AggregateException ex = new(new InvalidOperationException(), new InvalidCastException());

        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<InvalidOperationException>(() => throw ex));
    }

    [Fact]
    internal void Throws_AggregateWrongInternals()
    {
        AggregateException ex = new(new InvalidOperationException());

        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.Throws<InvalidCastException>(() => throw ex));
    }

    [Fact]
    internal void IsEmpty_Works()
    {
        _testInstance.IsEmpty(Array.Empty<string>());

        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.IsEmpty(null));
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.IsEmpty(Tools.Randomizer.Create<string[]>()));
    }

    [Fact]
    internal void IsNotEmpty_Works()
    {
        _fakeValuer.Setup(
            m => m.Equals(true, true),
            Behavior.Returns(true));
        _fakeValuer.Setup(
            m => m.Compare(true, Arg.Any<bool?>()),
            Behavior.Set((object o1, object o2) =>
            {
                return (!o1.Equals(o2))
                    ? Tools.Randomizer.Create<IEnumerable<Difference>>()
                    : Enumerable.Empty<Difference>();
            }));

        _testInstance.IsNotEmpty(Tools.Randomizer.Create<string[]>());

        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.IsNotEmpty(null));
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.IsNotEmpty(Array.Empty<string>()));
    }

    [Theory, RandomData]
    internal void HasCount_Works(IEnumerable<string> data)
    {
        _testInstance.HasCount(data.Count(), data);

        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.HasCount(data.Count(), null));
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.HasCount(data.Count() - 1, data));
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.HasCount(data.Count() + 1, data));
    }

    [Theory, RandomData]
    internal void ReferenceEqual_NotByValue(Fake<object> fake)
    {
        fake.Setup(
            m => m.Equals(Arg.Any<object>()),
            Behavior.Returns(true, Times.Never));

        _testInstance.ReferenceEqual(fake.Dummy, fake.Dummy);
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.ReferenceEqual(fake.Dummy, Tools.Duplicator.Copy(fake.Dummy)));

        fake.VerifyAll(Times.Never);
    }

    [Theory, RandomData]
    internal void ReferenceNotEqual_NotByValue(Fake<object> fake)
    {
        fake.Setup(
            m => m.Equals(Arg.Any<object>()),
            Behavior.Returns(false, Times.Never));

        _testInstance.ReferenceNotEqual(fake.Dummy, Tools.Duplicator.Copy(fake.Dummy));
        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.ReferenceNotEqual(fake.Dummy, fake.Dummy));

        fake.VerifyAll(Times.Never);
    }

    [Fact]
    internal void ValuesEqual_EqualValid()
    {
        _fakeValuer.Setup(
            m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
            Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));

        _testInstance.ValuesEqual(new object(), new object());

        _fakeValuer.VerifyAll(Times.Once);
    }

    [Fact]
    internal void ValuesEqual_UnequalInvalid()
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

    [Theory, RandomData]
    internal void ValuesEqual_CanHandleNullsNotEqual(IEnumerable<Difference> differences)
    {
        _fakeValuer.Setup(
            f => f.Compare(null, null),
            Behavior.Returns(differences, Times.Once));

        Tools.Asserter.Throws<AssertException>(
            () => _testInstance.ValuesEqual(null, null));

        _fakeValuer.VerifyAll(Times.Once);
    }

    [Fact]
    internal void ValuesNotEqual_EqualInvalid()
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

    [Fact]
    internal void ValuesNotEqual_UnequalValid()
    {
        _fakeValuer.Setup(
            m => m.Compare(Arg.Any<object>(), Arg.Any<object>()),
            Behavior.Returns(Tools.Randomizer.Create<IEnumerable<Difference>>(), Times.Once));

        _testInstance.ValuesNotEqual(new object(), new object());

        _fakeValuer.VerifyAll(Times.Once);
    }
}
