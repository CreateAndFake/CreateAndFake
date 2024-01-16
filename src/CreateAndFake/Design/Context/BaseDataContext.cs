using System;
using CreateAndFake.Design.Randomization;

namespace CreateAndFake.Design.Context;

/// <summary>Generates random values from data pools.</summary>
public abstract class BaseDataContext
{
    /// <inheritdoc cref="IRandom" />
    protected IRandom Gen { get; }

    /// <summary>Initializes a new instance of the <see cref="BaseDataContext"/> class.</summary>
    /// <param name="gen">Manages the randomization process.</param>
    internal BaseDataContext(IRandom gen)
    {
        Gen = gen ?? throw new ArgumentNullException(nameof(gen));
    }
}