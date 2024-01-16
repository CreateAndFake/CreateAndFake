using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace CreateAndFake;

/// <summary>Populates theory data with random values.</summary>
/// <remarks>Only available when using xUnit.</remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RandomDataAttribute : DataAttribute
{
    /// <summary>Number of times to test the method.</summary>
    public int Trials { get; set; } = 1;

    /// <summary>Generates data for a theory test.</summary>
    /// <param name="testMethod">Theory method details.</param>
    /// <returns>The generated data to run the test with.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        return Enumerable.Range(0, Math.Max(0, Trials))
            .Select(_ => Tools.Randomizer.CreateFor(testMethod).Args.ToArray());
    }
}
