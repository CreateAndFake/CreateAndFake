﻿using CreateAndFake.Design;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFakeTests.TestSamples;

public class ValuerEquatableSample : IValuerEquatable
{
    public string StringValue { get; set; }

    public int NumberValue;

    public virtual bool ValuesEqual(object other, IValuer valuer)
    {
        ArgumentGuard.ThrowIfNull(valuer, nameof(valuer));

        return (other is ValuerEquatableSample sample)
            && valuer.Equals(StringValue, sample.StringValue)
            && valuer.Equals(NumberValue, sample.NumberValue);
    }

    public virtual int GetValueHash(IValuer valuer)
    {
        return valuer?.GetHashCode(StringValue, NumberValue) ?? throw new ArgumentNullException(nameof(valuer));
    }
}
