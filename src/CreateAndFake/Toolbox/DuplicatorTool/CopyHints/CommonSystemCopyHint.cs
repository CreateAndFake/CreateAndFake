using System;
using System.Reflection;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying common types for the duplicator.</summary>
    public sealed class CommonSystemCopyHint : CopyHint
    {
        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal override sealed (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return (true, null);

            if (source is TimeSpan span)
            {
                return (true, new TimeSpan(duplicator.Copy(span.Ticks)));
            }
            else if (source is WeakReference reference)
            {
                return (true, new WeakReference(reference.Target, reference.TrackResurrection));
            }
            else if (source is MemberInfo member)
            {
                return (true, member);
            }
            else
            {
                return (false, null);
            }
        }
    }
}
