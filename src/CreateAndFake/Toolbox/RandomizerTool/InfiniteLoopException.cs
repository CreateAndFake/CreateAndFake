using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Exception when encountering infinite loops.</summary>
    [Serializable, KnownType(typeof(Exception)), KnownType(typeof(Exception[]))]
    public sealed class InfiniteLoopException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="InfiniteLoopException"/> class.</summary>
        /// <remarks>Serialization constructor.</remarks>
        private InfiniteLoopException() : base() { }

        /// <summary>Initializes a new instance of the <see cref="InfiniteLoopException"/> class.</summary>
        /// <param name="lastType">Last type attempted to be created.</param>
        /// <param name="history">Stack of created parents.</param>
        public InfiniteLoopException(Type lastType, IEnumerable<Type> history)
            : base($"Already created type '{lastType?.FullName}'. History:" +
                Environment.NewLine + string.Join(Environment.NewLine, history.Select(t => t.FullName)))
        { }

        /// <summary>Initializes a new instance of the <see cref="InfiniteLoopException"/> class.</summary>
        /// <param name="info">Object data.</param>
        /// <param name="context">Additional info.</param>
        /// <remarks>Serialization constructor.</remarks>
        private InfiniteLoopException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
