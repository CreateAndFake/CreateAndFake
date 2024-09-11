using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <inheritdoc/>
public sealed class AssertError : AssertErrorBase<AssertError>
{
    /// <inheritdoc/>
    internal AssertError(IRandom gen, IValuer valuer, Exception? error) : base(gen, valuer, error) { }
}

