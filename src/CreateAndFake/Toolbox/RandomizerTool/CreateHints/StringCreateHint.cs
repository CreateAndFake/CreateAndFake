using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles generation of strings for the randomizer.</summary>
/// <remarks>Initializes a new instance of the <see cref="StringCreateHint"/> class.</remarks>
/// <param name="minSize">Minimum size for created collections.</param>
/// <param name="range">Size variance for created collections.</param>
/// <param name="charSet">Character set to use for randomization.</param>
public sealed class StringCreateHint(int minSize = 7, int range = 5, IEnumerable<char> charSet = null)
    : CreateHint<string>
{
    /// <summary>Default characters to include in random strings.</summary>
    private static readonly char[] _DefaultCharSet =
        @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    /// <summary>Characters to include in random strings.</summary>
    private readonly char[] _charSet = (charSet?.Any() ?? false)
        ? charSet.ToArray()
        : _DefaultCharSet;

    /// <inheritdoc/>
    protected override string Create(RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        char[] data = new char[minSize + randomizer.Gen.Next(range)];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = randomizer.Gen.NextItem(_charSet);
        }
        return new string(data);
    }
}
