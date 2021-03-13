namespace CreateAndFake.Toolbox.DuplicatorTool
{
    /// <summary>Handles copying specific types for the duplicator.</summary>
    public abstract class CopyHint
    {
        /// <summary>Tries to deep clone <paramref name="source"/>.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>
        ///     (<c>true</c>, clone of <paramref name="source"/>) if successful;
        ///     (<c>false</c>, <c>null</c>) otherwise.
        /// </returns>
        protected internal abstract (bool, object) TryCopy(object source, DuplicatorChainer duplicator);
    }
}
