using System;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying cloneables for the duplicator.</summary>
    public sealed class CloneableCopyHint : CopyHint<ICloneable>
    {
        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Duplicator to handle child values.</param>
        /// <returns>Duplicate object.</returns>
        protected override ICloneable Copy(ICloneable source, IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return null;

            return (ICloneable)source.Clone();
        }
    }
}
