namespace CreateAndFake.Toolbox.RandomizerTool;

/// <summary>Handles randomizing specific types for <see cref="IRandomizer"/>.</summary>
public abstract class CreateHint
{
    /// <summary>Tries to create a random instance of the given <paramref name="type"/>.</summary>
    /// <param name="type"><c>Type</c> to generate.</param>
    /// <param name="randomizer">Handles randomizing child values.</param>
    /// <returns>
    ///     (<c>true</c>, created instance) if successful;
    ///     (<c>false</c>, <c>null</c>) otherwise.
    /// </returns>
    protected internal abstract (bool, object?) TryCreate(Type type, RandomizerChainer randomizer);
}
