using System.Collections;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <summary>Allows assertion calls to be chained.</summary>
/// <typeparam name="T">Assertion type to chain.</typeparam>
/// <param name="chain">Assertion class to chain.</param>
/// <param name="gen">Core value random handler.</param>
/// <param name="valuer">Handles comparisons.</param>
public sealed class AssertChainer<T>(T chain, IRandom gen, IValuer valuer)
{
    /// <summary>Includes another assertion on the value.</summary>
    public T And { get; } = chain;

    /// <summary>Specifies another value to test.</summary>
    /// <param name="actual">Object to test.</param>
    /// <returns>Asserter to test with.</returns>
    public AssertObject Also(object actual)
    {
        return new AssertObject(gen, valuer, actual);
    }

    /// <summary>Specifies another value to test.</summary>
    /// <param name="collection">Collection to test.</param>
    /// <returns>Asserter to test with.</returns>
    public AssertGroup Also(IEnumerable collection)
    {
        return new AssertGroup(gen, valuer, collection);
    }
}
