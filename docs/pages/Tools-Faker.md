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

* Arg - Useful for matching parameters by various conditions.
* Behavior - Used to define what happens when the mocked method is called.
* Times - Specifies expected number of calls.

## Creation & Customization

The Faker requires a Valuer to be given at creation, which controls value equality behavior for arg matching.
