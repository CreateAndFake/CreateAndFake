namespace CreateAndFake.Toolbox.DuplicatorTool;

/// <typeparam name="T">Type to handle.</typeparam>
/// <inheritdoc/>
public abstract class CopyHint<T> : CopyHint
{
    /// <inheritdoc/>
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

    /// <summary>Deep clones <paramref name="source"/>.</summary>
    /// <param name="source">Object to clone.</param>
    /// <param name="duplicator">Handles callback behavior for child values.</param>
    /// <returns>Clone of <paramref name="source"/>.</returns>
    protected abstract T Copy(T source, DuplicatorChainer duplicator);
}
