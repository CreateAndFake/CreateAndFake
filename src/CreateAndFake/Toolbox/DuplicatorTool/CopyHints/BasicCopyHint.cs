using System;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying specific types for the duplicator.</summary>
    public sealed class BasicCopyHint : CopyHint
    {
        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Duplicator to handle child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal override sealed (bool, object) TryCopy(object source, IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            Type type = source?.GetType();
            if (type == null
                || type.IsPrimitive
                || type.IsEnum
                || type == typeof(object)
                || type == typeof(String))
            {
                return (true, source);
            }
            else
            {
                return (false, null);
            }
        }
    }
}
