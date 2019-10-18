using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing tasks for the valuer.</summary>
    public sealed class TaskCompareHint : CompareHint<Task>
    {
        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(Task expected, Task actual, ValuerChainer valuer)
        {
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return valuer.Compare(ExtractResult(expected), ExtractResult(actual));
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(Task item, ValuerChainer valuer)
        {
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return valuer.GetHashCode(ExtractResult(item));
        }

        /// <summary>Retrives the result from a task.</summary>
        /// <param name="item">Given task.</param>
        /// <returns>Result if possible; status otherwise.</returns>
        private static object ExtractResult(Task item)
        {
            if (item == null) return null;

            if (item.GetType().IsGenericType)
            {
                return item
                    .GetType()
                    .GetProperty(nameof(Task<object>.Result))
                    .GetValue(item);
            }
            else
            {
                return (item.Status, item.Exception);
            }
        }
    }
}
