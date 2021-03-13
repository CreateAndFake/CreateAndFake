using System;
using System.Runtime.Serialization;

namespace CreateAndFake.Toolbox.AsserterTool
{
    /// <summary>Exception for a failed asserter case.</summary>
    [Serializable, KnownType(typeof(Exception)), KnownType(typeof(Exception[]))]
    public sealed class AssertException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="AssertException"/> class.</summary>
        /// <remarks>Serialization constructor.</remarks>
        private AssertException() : base() { }

        /// <summary>Initializes a new instance of the <see cref="AssertException"/> class.</summary>
        /// <param name="message">Reason for the exception.</param>
        /// <param name="details">Optional fail details.</param>
        /// <param name="seed">Seed used for data generation.</param>
        /// <param name="content">Optional related content.</param>
        public AssertException(string message, string details, int? seed, string content = null)
            : base(BuildMessage(message, details, seed, content)) { }

        /// <summary>Initializes a new instance of the <see cref="AssertException"/> class.</summary>
        /// <param name="message">Reason for the exception.</param>
        /// <param name="details">Optional fail details.</param>
        /// <param name="seed">Seed used for data generation.</param>
        /// <param name="innerException">Inner exception that occurred.</param>
        public AssertException(string message, string details, int? seed, Exception innerException)
            : base(BuildMessage(message, details, seed), innerException) { }

        /// <summary>Initializes a new instance of the <see cref="AssertException"/> class.</summary>
        /// <param name="message">Reason for the exception.</param>
        /// <param name="details">Optional fail details.</param>
        /// <param name="seed">Seed used for data generation.</param>
        /// <param name="content">Optional related content.</param>
        /// <param name="innerException">Inner exception that occurred.</param>
        public AssertException(string message, string details, int? seed, string content, Exception innerException)
            : base(BuildMessage(message, details, seed, content), innerException) { }

        /// <summary>Initializes a new instance of the <see cref="AssertException"/> class.</summary>
        /// <param name="info">Object data.</param>
        /// <param name="context">Additional info.</param>
        /// <remarks>Serialization constructor.</remarks>
        private AssertException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>Integrates the details into the message.</summary>
        /// <param name="message">Starting message.</param>
        /// <param name="details">Details to integrate.</param>
        /// <param name="seed">Seed used for data generation.</param>
        /// <param name="content">Content to integrate.</param>
        /// <returns>Message to use for the exception.</returns>
        private static string BuildMessage(string message, string details, int? seed, string content = null)
        {
            string nl = Environment.NewLine;

            return (message ?? "Unknown assert failure.") +
                ((details != null) ? $"{nl}Details: {details}" : "") +
                ((seed.HasValue) ? $"{nl}Seed: {seed.Value}" : "") +
                ((content != null) ? $"{nl}Content: {nl}{content}" : "");
        }
    }
}
