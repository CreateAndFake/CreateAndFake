namespace CreateAndFake.Toolbox.RandomizerTool;

/// <summary>Handles randomizing specific collections for <see cref="IRandomizer"/>.</summary>
public abstract class CreateCollectionHint : CreateHint
{
    /// <param name="size">Number of items to generate.</param>
    /// <inheritdoc cref="CreateHint.TryCreate(Type,RandomizerChainer)"/>
    protected internal abstract (bool, object?) TryCreate(Type type, int size, RandomizerChainer randomizer);
}
