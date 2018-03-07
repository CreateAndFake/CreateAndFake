# Asserter

## Usage

The Asserter tool provides methods to verify common test scenarios.

The most common are ensuring the size of a collection, verifying an
exception throws, and value quality checks. Here's an example of such
behavior in action:

```c#
/// <summary>Test that will fail.</summary>
[TestMethod]
public void Tools_DataSampleExample()
{
    DataSample original = Tools.Randomizer.Create<DataSample>();
    DataSample variant = Tools.Randiffer.Branch(original);
    Tools.Asserter.Is(original, variant);
}
```

Which gives the following message detailing the differences:

```
Result Message:
Test method CreateAndFakeTests.ToolsTests.Tools_DataSampleExample threw
exception:
CreateAndFake.Toolbox.AsserterTool.AssertException: Value equality failed
for type 'DataSample'.
Content:
.NestedValue.StringValue -> Expected:<CYTsi8X>, Actual:<0RIXwSBI>
.NestedValue.NumberValue -> Expected:<-1982040207>, Actual:<-1975491997>
```

## Creation & Customization

The Asserter requires a Valuer to be given at creation, which controls the
equality testing behavior.

Unlike the other tools, the Asserter can be subclassed to add new methods
or override any method to provide different behavior.
