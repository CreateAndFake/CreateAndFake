using System.Reflection;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Handles cloning <see cref="IAsyncEnumerable{T}"/> collections for <see cref="IDuplicator"/> .</summary>
public sealed class AsyncCollectionCopyHint : CopyHint
{
    /// <inheritdoc/>
    protected internal override (bool, object?) TryCopy(object source, DuplicatorChainer duplicator)
    {
        ArgumentGuard.ThrowIfNull(source, nameof(source));
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        if (source.GetType().Inherits(typeof(IAsyncEnumerable<>)))
        {
            return (true, typeof(AsyncCollectionCopyHint)
                .GetMethod(nameof(CopyAsync), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(source.GetType().GetGenericArguments().Single())
                .Invoke(null, [source, duplicator]));
        }
        else
        {
            return (false, null);
        }
    }

    /// <summary>Deep clones <paramref name="source"/>.</summary>
    /// <typeparam name="T">Item type being copied.</typeparam>
    /// <param name="source">Object to clone.</param>
    /// <param name="duplicator">Handles callback behavior for child values.</param>
    /// <returns>Clone of <paramref name="source"/>.</returns>
    private static async IAsyncEnumerable<T?> CopyAsync<T>(IAsyncEnumerable<T> source, DuplicatorChainer duplicator)
    {
        await foreach (T item in source)
        {
            yield return duplicator.Copy(item);
        }
    }
}