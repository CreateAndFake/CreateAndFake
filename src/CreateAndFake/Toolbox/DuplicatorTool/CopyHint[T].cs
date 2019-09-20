namespace CreateAndFake.Toolbox.DuplicatorTool
{
    /// <summary>Handles copying specific types for the duplicator.</summary>
    /// <typeparam name="T">Type to handle.</typeparam>
    public abstract class CopyHint<T> : CopyHint
    {
        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal sealed override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
        {
            if (source == null)
            {
                return (true, Copy(default, duplicator));
            }
            else if (source is T data)
            {
                return (true, Copy(data, duplicator));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Duplicate object.</returns>
        protected abstract T Copy(T source, DuplicatorChainer duplicator);
    }
}
