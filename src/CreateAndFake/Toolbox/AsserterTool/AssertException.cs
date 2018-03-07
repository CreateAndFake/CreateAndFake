using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace CreateAndFake.Toolbox.AsserterTool
{
    /// <summary>Exception for a failed asserter case.</summary>
    [Serializable, KnownType(typeof(Exception)), KnownType(typeof(Exception[]))]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "Enforces pattern behind the exception existing instead.")]
    public sealed class AssertException : Exception
    {
        /// <summary>Serialization cosntructor.</summary>
        private AssertException() : base() { }

        /// <summary>Sets up the exception.</summary>
        /// <param name="message">Reason for the exception.</param>
        /// <param name="details">Optional fail details.</param>
        /// <param name="content">Optional related content.</param>
        public AssertException(string message, string details, string content = null)
            : base(BuildMessage(message, details, content)) { }

        /// <summary>Sets up the exception.</summary>
        /// <param name="message">Reason for the exception.</param>
        /// <param name="details">Optional fail details.</param>
        /// <param name="innerException">Inner exception that occurred.</param>
        public AssertException(string message, string details, Exception innerException)
            : base(BuildMessage(message, details), innerException) { }

        /// <summary>Sets up the exception.</summary>
        /// <param name="message">Reason for the exception.</param>
        /// <param name="details">Optional fail details.</param>
        /// <param name="content">Optional related content.</param>
        /// <param name="innerException">Inner exception that occurred.</param>
        public AssertException(string message, string details, string content, Exception innerException)
            : base(BuildMessage(message, details, content), innerException) { }

        /// <summary>Serialization constructor.</summary>
        /// <param name="info">Object data.</param>
        /// <param name="context">Additional info.</param>
        private AssertException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>Integrates the details into the message.</summary>
        /// <param name="message">Starting message.</param>
        /// <param name="details">Details to integrate.</param>
        /// <param name="content">Content to integrate.</param>
        /// <returns>Message to use for the exception.</returns>
        private static string BuildMessage(string message, string details, string content = null)
        {
            return (message ?? "Unknown assert failure.") +
                ((details == null) ? "" : Environment.NewLine + "Details: " + details) +
                ((content == null) ? "" : Environment.NewLine + "Content:" + Environment.NewLine + content);
        }
    }
}
