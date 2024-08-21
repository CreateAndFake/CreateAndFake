using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <inheritdoc/>
public sealed class AssertComparable : AssertComparableBase<AssertComparable>
{
    /// <inheritdoc/>
    internal AssertComparable(IRandom gen, IValuer valuer, IComparable? value) : base(gen, valuer, value) { }
}
