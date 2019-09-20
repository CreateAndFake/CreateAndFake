using System;
using System.Runtime.Serialization;
using CreateAndFake.Design.Randomization;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying specific types for the duplicator.</summary>
    public sealed class BasicCopyHint : CopyHint
    {
        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal sealed override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
        {
            Type type = source?.GetType();
            if (type == null
                || type.IsPrimitive
                || type.IsEnum
                || ValueRandom.ValueTypes.Contains(type)
                || type == typeof(object)
                || type == typeof(string)
                || type.Inherits<IObjectReference>())
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
