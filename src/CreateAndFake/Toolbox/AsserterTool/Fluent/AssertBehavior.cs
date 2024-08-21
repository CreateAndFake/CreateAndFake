using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <inheritdoc/>
public sealed class AssertBehavior : AssertBehaviorBase<AssertBehavior>
{
    /// <inheritdoc/>
    internal AssertBehavior(IRandom gen, IValuer valuer, Delegate? behavior) : base(gen, valuer, behavior) { }
}
