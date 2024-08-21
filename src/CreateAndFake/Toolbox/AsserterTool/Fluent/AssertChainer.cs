using System.Collections;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <summary>Allows assertion calls to be chained fluently.</summary>
/// <typeparam name="T">Assertion base <c>Type</c> to chain.</typeparam>
/// <param name="chain">Assertion base instance to chain.</param>
/// <param name="gen">Core randomizer with a potential seed for logging.</param>
/// <param name="valuer">Handles comparisons for assertion checks.</param>
public sealed class AssertChainer<T>(T chain, IRandom gen, IValuer valuer)
{
    /// <summary>Includes another assertion on the instance to test.</summary>
    public T And { get; } = chain;

    /// <summary>Specifies a different instance to test.</summary>
    /// <param name="actual">Object to test.</param>
    /// <returns>Asserter to test with.</returns>
    public AssertObject Also(object? actual)
    {
        return new AssertObject(gen, valuer, actual);
    }

    /// <summary>Specifies a different instance to test.</summary>
    /// <param name="collection">Collection to test.</param>
    /// <returns>Asserter to test with.</returns>
    public AssertGroup Also(IEnumerable? collection)
    {
        return new AssertGroup(gen, valuer, collection);
    }
}
