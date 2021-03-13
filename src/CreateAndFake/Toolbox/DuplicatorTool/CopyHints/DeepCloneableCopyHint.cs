using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying deep cloneables for the duplicator.</summary>
    public sealed class DeepCloneableCopyHint : CopyHint<IDeepCloneable>
    {
        /// <inheritdoc/>
        protected override IDeepCloneable Copy(IDeepCloneable source, DuplicatorChainer duplicator)
        {
            return source?.DeepClone();
        }
    }
}
