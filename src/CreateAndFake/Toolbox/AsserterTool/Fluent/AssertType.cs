using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <inheritdoc/>
public sealed class AssertType : AssertTypeBase<AssertType>
{
    /// <inheritdoc/>
    internal AssertType(IRandom gen, IValuer valuer, Type? type) : base(gen, valuer, type) { }
}
