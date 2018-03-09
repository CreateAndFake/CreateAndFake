using System;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying cloneables for the duplicator.</summary>
    public sealed class CloneableCopyHint : CopyHint<ICloneable>
    {
        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Duplicate object.</returns>
        protected override ICloneable Copy(ICloneable source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return null;

            return (ICloneable)source.Clone();
        }
    }
}
