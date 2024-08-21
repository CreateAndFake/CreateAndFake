using System.Reflection;
using Xunit.Sdk;

namespace CreateAndFake;

/// <summary>Populates <see cref="Xunit.TheoryAttribute"/> data with random values.</summary>
/// <remarks>Only available when using <c>xUnit</c>.</remarks>
/// <seealso cref="Toolbox.RandomizerTool.IRandomizer.CreateFor"/>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RandomDataAttribute : DataAttribute
{
    /// <summary>Number of times to test the method.</summary>
    public int Trials { get; set; } = 1;

    /// <summary>Generates data for a <see cref="Xunit.TheoryAttribute"/> test.</summary>
    /// <param name="testMethod">Method details <c>this</c> is attached to.</param>
    /// <returns>The generated data to run the test with.</returns>
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
        return Enumerable.Range(0, Math.Max(0, Trials))
            .Select(_ => Tools.Randomizer.CreateFor(testMethod).Args.ToArray());
    }
}
