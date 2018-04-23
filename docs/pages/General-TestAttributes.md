# Test Attributes

For many test frameworks, attributes can create parameter data for test methods. For example, using xUnit:

```c#
/// <summary>Verifies the option works.</summary>
[Theory, RandomData]
public static void Min_Works(int value)
{
    Tools.Asserter.Is(false, Times.Min(value).IsInRange(value - 1));
    Tools.Asserter.Is(true, Times.Min(value).IsInRange(value));
    Tools.Asserter.Is(true, Times.Min(value).IsInRange(value + 1));
}
```

Using the following attribute:

```c#
/// <summary>Populates theory data with random values.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RandomDataAttribute : DataAttribute
{
    /// <summary>Number of times to test the method.</summary>
    public int Trials { get; set; } = 1;

    /// <summary>Generates data for a theory.</summary>
    /// <param name="testMethod">Theory information.</param>
    /// <returns>The generated data.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null) throw new ArgumentNullException(nameof(testMethod));

        object[][] result = new object[Math.Max(Trials, 0)][];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = testMethod.GetParameters().Select(p => Tools.Randomizer.Create(p.ParameterType)).ToArray();
        }
        return result;
    }
}
```
