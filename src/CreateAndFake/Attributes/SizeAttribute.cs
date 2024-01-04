using System;

namespace CreateAndFake;

/// <summary>Flag to specify size of created collection.</summary>
/// <param name="count">Number of items to generate.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class SizeAttribute(int count) : Attribute
{
    /// <summary>Number of items to generate.</summary>
    public int Count { get; } = count;
}