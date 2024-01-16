using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <inheritdoc/>
public sealed class AssertText : AssertTextBase<AssertText>
{
    /// <summary>Initializes a new instance of the <see cref="AssertText"/> class.</summary>
    /// <inheritdoc/>
    internal AssertText(IRandom gen, IValuer valuer, string text) : base(gen, valuer, text) { }
}
