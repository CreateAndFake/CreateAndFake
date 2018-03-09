namespace CreateAndFake.Toolbox.DuplicatorTool
{
    /// <summary>Handles copying specific types for the duplicator.</summary>
    public abstract class CopyHint
    {
        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal abstract (bool, object) TryCopy(object source, DuplicatorChainer duplicator);
    }
}
