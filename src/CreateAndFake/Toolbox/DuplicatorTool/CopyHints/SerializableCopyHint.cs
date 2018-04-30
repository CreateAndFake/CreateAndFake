using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying serializables for the duplicator.</summary>
    public sealed class SerializableCopyHint : CopyHint
    {
        /// <summary>Handles the serialization process.</summary>
        private static readonly IFormatter s_Serializer = new BinaryFormatter();

        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
        {
            if (source == null) return (true, null);

            if (source is ISerializable && source.GetType().IsSerializable)
            {
                using (Stream stream = new MemoryStream())
                {
                    s_Serializer.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (true, s_Serializer.Deserialize(stream));
                }
            }
            else
            {
                return (false, null);
            }
        }
    }
}
