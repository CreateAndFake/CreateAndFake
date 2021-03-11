using System;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <inheritdoc/>
    public sealed class AssertBehavior : AssertBehaviorBase<AssertBehavior>
    {
        /// <summary>Initializes a new instance of the <see cref="AssertBehavior"/> class.</summary>
        /// <inheritdoc/>
        internal AssertBehavior(IRandom gen, IValuer valuer, Delegate behavior) : base(gen, valuer, behavior) { }
    }
}
