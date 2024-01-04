using System;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFakeTests.TestSamples;

/// <summary>For testing.</summary>
/// <remarks>For testing.</remarks>
/// <param name="stringValue">For testing.</param>
public class PrivateValuerEquatableSample(string stringValue) : IValuerEquatable
{
    /// <summary>For testing.</summary>
    private string StringValue { get; } = stringValue;

    /// <summary>Compares by value.</summary>
    /// <param name="other">Instance to compare with.</param>
    /// <param name="valuer">Handles callback behavior for child values.</param>
    /// <returns>True if equal; false otherwise.</returns>
    public virtual bool ValuesEqual(object other, IValuer valuer)
    {
        ArgumentGuard.ThrowIfNull(valuer, nameof(valuer));

        return (other is PrivateValuerEquatableSample sample)
            && valuer.Equals(StringValue, sample.StringValue);
    }

    /// <summary>Generates a hash based upon value.</summary>
    /// <param name="valuer">Handles callback behavior for child values.</param>
    /// <returns>The generated hash code.</returns>
    public virtual int GetValueHash(IValuer valuer)
    {
        return valuer?.GetHashCode(StringValue) ?? throw new ArgumentNullException(nameof(valuer));
    }
}
