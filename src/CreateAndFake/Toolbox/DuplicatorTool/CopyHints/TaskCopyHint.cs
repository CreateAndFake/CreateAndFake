using System;
using System.Linq;
using System.Threading.Tasks;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying tasks for the duplicator.</summary>
    public sealed class TaskCopyHint : CopyHint
    {
        /// <inheritdoc/>
        protected internal sealed override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return (true, null);

            if (source is Task task && task.GetType().IsGenericType)
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
        private static Task Copy(Task source, DuplicatorChainer duplicator)
        {
            Type taskType = source.GetType();

            object content = taskType.GetProperty(nameof(Task<object>.Result)).GetValue(source);

            return (Task)typeof(Task)
                .GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(taskType.GetGenericArguments().Single())
                .Invoke(null, new[] { duplicator.Copy(content) });
        }
    }
}
