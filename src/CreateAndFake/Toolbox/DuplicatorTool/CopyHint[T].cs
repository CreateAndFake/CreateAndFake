namespace CreateAndFake.Toolbox.DuplicatorTool;

/// <typeparam name="T"><c>Type</c> being supported for cloning.</typeparam>
/// <inheritdoc/>
public abstract class CopyHint<T> : CopyHint
{
    /// <inheritdoc/>
    protected internal sealed override (bool, object?) TryCopy(object source, DuplicatorChainer duplicator)
    {
        if (source is T data)
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
    /// <param name="duplicator">Handles cloning child values.</param>
    /// <returns>Clone of <paramref name="source"/>.</returns>
    protected abstract T Copy(T source, DuplicatorChainer duplicator);
}
