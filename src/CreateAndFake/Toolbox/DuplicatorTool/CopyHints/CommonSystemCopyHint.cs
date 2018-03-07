using System;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying common types for the duplicator.</summary>
    public sealed class CommonSystemCopyHint : CopyHint
    {
        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Duplicator to handle child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal override sealed (bool, object) TryCopy(object source, IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return (true, null);

            if (source is TimeSpan data)
            {
                return (true, new TimeSpan(((TimeSpan)source).Ticks));
            }
            else
            {
                return (false, null);
            }
        }
    }
}
