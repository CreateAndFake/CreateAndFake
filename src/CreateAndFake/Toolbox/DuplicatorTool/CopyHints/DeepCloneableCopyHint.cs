using System;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying deep cloneables for the duplicator.</summary>
    public sealed class DeepCloneableCopyHint : CopyHint<IDeepCloneable>
    {
        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Duplicate object.</returns>
        protected override IDeepCloneable Copy(IDeepCloneable source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return source?.DeepClone();
        }
    }
}
