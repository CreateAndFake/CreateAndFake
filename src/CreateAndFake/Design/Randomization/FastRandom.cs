﻿using System;

#pragma warning disable CA5394 // Do not use insecure randomness

namespace CreateAndFake.Design.Randomization;

/// <summary>For generating quick but insecure random values.</summary>
/// <inheritdoc/>
public sealed class FastRandom(bool onlyValidValues = true) : ValueRandom(onlyValidValues)
{
    /// <inheritdoc/>
    public override int? InitialSeed { get; } = null;

    /// <summary>Source generator used for random bytes.</summary>
    private static readonly Random _Gen = new();

    /// <inheritdoc/>
    protected override byte[] NextBytes(short length)
    {
        byte[] buffer = new byte[length];
        lock (_Gen)
        {
            _Gen.NextBytes(buffer);
        }
        return buffer;
    }
}

#pragma warning restore CA5394 // Do not use insecure randomness
