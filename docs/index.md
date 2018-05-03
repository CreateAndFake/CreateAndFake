# Getting Started

Welcome to the documentation source for developers. 

## Quick Start

A default set of all tools are ready to use under the static class Tools.

```c#
/// <summary>Verifies that the tools integrate together.</summary>
[Fact]
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

The tools are designed to cover many common scenarios including immutable classes, classes with private fields, and classes with static factories instead of constructors.

## Customization

In case the default behavior is not enough, each tool can be customized for individual needs by creating your own instances. Additionally, interfaces have been provided for data classes to properly behave with the tools instead. Visit any tool's page to be guided on either route.

Although each tool can be created and customized on a per test basis, it is recommended to create a your own static class containing the tools instead of the supplied default:

```c#
/// <summary>Holds basic implementations of all reflection tools.</summary>
public static class Tool
{
    /// <summary>Core value random handler.</summary>
    public static IRandom Gen { get; } = new FastRandom();

    /// <summary>Compares objects by value.</summary>
    public static IValuer Valuer { get; } = new Valuer();

    /// <summary>Creates fake objects.</summary>
    public static IFaker Faker { get; } = new Faker(Valuer);

    /// <summary>Creates objects and populates them with random values.</summary>
    public static IRandomizer Randomizer { get; } = new Randomizer(Faker, Gen);

    /// <summary>Creates random variants of objects.</summary>
    public static IRandiffer Randiffer { get; } = new Randiffer(Randomizer, Valuer, Limiter.Dozen);

    /// <summary>Handles common test scenarios.</summary>
    public static Asserter Asserter { get; } = new Asserter(Valuer);

    /// <summary>Deep clones objects.</summary>
    public static IDuplicator Duplicator { get; } = new Duplicator(Asserter);

    /// <summary>Automates common tests.</summary>
    public static Tester Tester { get; } = new Tester(Gen, Randomizer, Duplicator, Asserter);
}
```

This way, any customization changes are picked up throughout your code.
