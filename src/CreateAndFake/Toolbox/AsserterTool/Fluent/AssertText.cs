using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <inheritdoc/>
public sealed class AssertText : AssertTextBase<AssertText>
{
    /// <inheritdoc/>
    internal AssertText(IRandom gen, IValuer valuer, string? text) : base(gen, valuer, text) { }
}
