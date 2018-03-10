# Faker

## Usage

The Faker tool provides mocks and stubs in a typical lambda based style with a very methodical structure:

```c#
/// <summary>Verifies setup functionality.</summary>
[TestMethod]
public void Setup_ObjectEquality()
{
    object data = new object();

    Fake<object> fake = Tools.Faker.Mock<object>();
    fake.Setup(
        d => d.Equals(Arg.Any<object>()),
        Behavior.Set((object o) => { return false; }));
    fake.Setup(
        d => d.Equals(data),
        Behavior.Returns(true, Times.Once));

    Tools.Asserter.Is(false, fake.Dummy.Equals(new object()));
    Tools.Asserter.Is(true, fake.Dummy.Equals(data));

    fake.Verify(Times.Exactly(2));
}
```

The Faker functionality is based upon only a few components:

* `Arg` - Useful for matching parameters by various conditions.
* `Behavior` - Used to define what happens when the mocked method is called.
* `Times` - Specifies expected number of calls.

## Creation & Customization

The Faker requires a Valuer to be given at creation, which controls value equality behavior for parameter matching.

## Verifying

There are a variety of options to verify methods were called as expected:

* `Fake.Verify()` - Automatically checks methods based upon Times in each Behavior.
* `Fake.Verify(Times)` - Does the above and that the total matches the provided Times.
* `Fake.Verify(Times, delegate)` - Verifies the given delegate was called the given Times.
* `Fake.Verify(Times, CallData)` - Verifies the given CallData was called the given Times.

Note that CallData is the returned result of a setup call, which can typically be ignored unless you want to use it for verification.

## Faking Internal

The assembly containing the internals to fake needs to grant visibility to the dynamic assembly by adding somewhere (typically AssemblyInfo.cs):

```c#
[assembly: InternalsVisibleTo("FakerTypes")]
```

## Faking Protected

The fakes can be setup using the string names for protected methods with an array containing the parameters to match. However, note that using Arg conditions requires the .Lambda versions to successfully match calls.

```c#
/// <summary>Verifies hints generate the hashes.</summary>
[TestMethod]
public void GetHashCode_ValidHint()
{
    object data = new object();
    int result = Tools.Randomizer.Create<int>();

    Fake<CompareHint> hint = Tools.Faker.Mock<CompareHint>();
    hint.Setup("Supports",
        new[] { data, data, Arg.LambdaAny<ValuerChainer>() },
        Behavior.Returns(true, Times.Once));
    hint.Setup("GetHashCode",
        new[] { data, Arg.LambdaAny<ValuerChainer>() },
        Behavior.Returns(result, Times.Once));

    Tools.Asserter.Is(result, new Valuer(false, hint.Dummy).GetHashCode(data));
    hint.Verify(Times.Exactly(2));
}
```
