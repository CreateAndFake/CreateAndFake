using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Exception when encountering infinite loops.</summary>
    [Serializable, KnownType(typeof(Exception)), KnownType(typeof(Exception[]))]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "Enforces pattern behind the exception existing instead.")]
    public sealed class InfiniteLoopException : Exception
    {
        /// <summary>Serialization cosntructor.</summary>
        private InfiniteLoopException() : base() { }

        /// <summary>Sets up the exception.</summary>
        /// <param name="lastType">Last type attempted to be created.</param>
        /// <param name="history">Stack of created parents.</param>
        public InfiniteLoopException(Type lastType, IEnumerable<Type> history)
            : base("Already created type '" + lastType.FullName + "'. History:" +
                Environment.NewLine + String.Join(Environment.NewLine, history.Select(t => t.FullName)))
        { }

        /// <summary>Serialization constructor.</summary>
        /// <param name="info">Object data.</param>
        /// <param name="context">Additional info.</param>
        private InfiniteLoopException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
