using System.Reflection;
using System.Threading.Tasks;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Handles copying tasks for the duplicator.</summary>
public sealed class TaskCopyHint : CopyHint
{
    /// <inheritdoc/>
    protected internal sealed override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
    {
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        if (source == null)
        {
            return (true, null);
        }
        else if (source is Task task && task.GetType().IsGenericType)
        {
            return (true, Copy(task, duplicator));
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
    private static object Copy(Task source, DuplicatorChainer duplicator)
    {
        PropertyInfo resultHolder = source.GetType().GetProperty(nameof(Task<object>.Result));

        return typeof(TaskCopyHint)
            .GetMethod(nameof(NewInstanceFromResult), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(resultHolder.PropertyType)
            .Invoke(null, [duplicator.Copy(resultHolder.GetValue(source))]);
    }

    /// <summary>Forces a task instance that isn't cached.</summary>
    /// <typeparam name="T">Task result type.</typeparam>
    /// <param name="result">Value for the task</param>
    /// <returns>New task instance.</returns>
    private static Task<T> NewInstanceFromResult<T>(T result)
    {
        TaskCompletionSource<T> wrapper = new();
        _ = wrapper.TrySetResult(result);
        return wrapper.Task;
    }
}
