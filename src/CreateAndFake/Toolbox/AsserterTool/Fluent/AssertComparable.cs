using System;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <inheritdoc/>
public sealed class AssertComparable : AssertComparableBase<AssertComparable>
{
    /// <summary>Initializes a new instance of the <see cref="AssertComparable"/> class.</summary>
    /// <inheritdoc/>
    internal AssertComparable(IRandom gen, IValuer valuer, IComparable value) : base(gen, valuer, value) { }
}
