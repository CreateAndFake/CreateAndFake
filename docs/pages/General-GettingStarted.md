# Getting Started

A default set of all tools are ready to use under the static class Tools.

```c#
/// <summary>Verifies that the tools integrate together.</summary>
[TestMethod]
public void Tools_IntegrationWorks()
{
    DataSample original = Tools.Randomizer.Create<DataSample>();
    DataSample variant = Tools.Randiffer.Branch(original);
    DataSample dupe = Tools.Duplicator.Copy(original);

    Tools.Asserter.Is(original, dupe);
    Tools.Asserter.IsNot(original, variant);

    Fake<DataSample> faked = Tools.Faker.Mock<DataSample>();
    faked.Setup(
        d => d.HasNested(dupe),
        Behavior.Returns(true, Times.Once));

    Tools.Asserter.Is(true, faked.Dummy.HasNested(original),
        "Value equality did not work for args.");

    faked.Verify(Times.Once);
}
```

The tools are designed to cover many common scenarios including immutable classes, classes with private fields, and classes with static factories instead of constructors. However, in case it's not enough, each tool can be customized for individual needs not covered by creating your own instances. Additionally, interfaces have been provided for data classes to properly behave with the tools instead. Visit any tool's page to be guided on either route.
