using System;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying duplicatables for the duplicator.</summary>
    public sealed class DuplicatableCopyHint : CopyHint<IDuplicatable>
    {
        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Duplicate object.</returns>
        protected override IDuplicatable Copy(IDuplicatable source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return source?.DeepClone(duplicator.Duplicator);
        }
    }
}
