using System;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying duplicatables for the duplicator.</summary>
    public sealed class DuplicatableCopyHint : CopyHint<IDuplicatable>
    {
        /// <inheritdoc/>
        protected override IDuplicatable Copy(IDuplicatable source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return source?.DeepClone(duplicator.Duplicator);
        }
    }
}
