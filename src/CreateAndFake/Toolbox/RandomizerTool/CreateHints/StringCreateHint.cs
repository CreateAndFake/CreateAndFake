using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles randomizing <see cref="string"/> collections for <see cref="IRandomizer"/>.</summary>
/// <param name="charSet"><inheritdoc cref="_charSet" path=""/></param>
/// <inheritdoc cref="CollectionCreateHint"/>
public sealed class StringCreateHint(int minSize = 7, int range = 5, IEnumerable<char>? charSet = null)
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
