using System;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Handles copying cloneables for the duplicator.</summary>
public sealed class CloneableCopyHint : CopyHint<ICloneable>
{
    /// <inheritdoc/>
    protected override ICloneable Copy(ICloneable source, DuplicatorChainer duplicator)
    {
        return (ICloneable)source?.Clone();
    }
}
