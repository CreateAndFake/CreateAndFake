# Randiffer

## Usage

The Randiffer tool provides methods to create variants.

The most common scenario to use this behavior is verifying a method's behavior changes based upon input. For example:

```c#
/// <summary>Verifies different hashes are generated for different values.</summary>
[TestMethod]
public void GetHashCode_HashVaries()
{
    int original = Tools.Randomizer.Create<int>();
    int variant = Tools.Randiffer.Branch(original);

    Tools.Asserter.ValuesNotEqual(
        ValueComparer.Use.GetHashCode(original),
        ValueComparer.Use.GetHashCode(variant));
}
```

## Creation & Customization

The Randiffer requires a Randomizer to be given at creation, which controls the actual creation and randomization of object, a Valuer, which verifies the created variant is unequal to the original by value, and a Limiter, which limits the attempts at creating variants.

At the moment, Randiffer completely relies upon other tools for its behavior. In the future as the behavior expands, it will work like other tools in that it can be customized by hints.
