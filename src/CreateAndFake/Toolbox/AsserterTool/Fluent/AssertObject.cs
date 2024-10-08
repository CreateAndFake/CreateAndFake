﻿using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <inheritdoc/>
public sealed class AssertObject : AssertObjectBase<AssertObject>
{
    /// <inheritdoc/>
    internal AssertObject(IRandom gen, IValuer valuer, object? actual) : base(gen, valuer, actual) { }
}
