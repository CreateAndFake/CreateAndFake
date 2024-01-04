using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Handles copying duplicatables for the duplicator.</summary>
public sealed class DuplicatableCopyHint : CopyHint<IDuplicatable>
{
    /// <inheritdoc/>
    protected override IDuplicatable Copy(IDuplicatable source, DuplicatorChainer duplicator)
    {
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        return source?.DeepClone(duplicator.Duplicator);
    }
}
