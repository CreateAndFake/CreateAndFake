using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CreateAndFake.Toolbox.FakerTool.Proxy;

/// <summary>Exception for a call lacking behavior.</summary>
[Serializable, KnownType(typeof(Exception))]
public sealed class FakeCallException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="FakeCallException"/> class.</summary>
    /// <remarks>Serialization constructor.</remarks>
    private FakeCallException() : base() { }

    /// <summary>Initializes a new instance of the <see cref="FakeCallException"/> class.</summary>
    /// <param name="data">Associated call data.</param>
    /// <param name="setup">Call data with behavior.</param>
    internal FakeCallException(CallData data, IEnumerable<CallData> setup) : base(BuildMessage(data, setup)) { }

    /// <summary>Initializes a new instance of the <see cref="FakeCallException"/> class.</summary>
    /// <param name="info">Object data.</param>
    /// <param name="context">Additional info.</param>
    /// <remarks>Serialization constructor.</remarks>
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    private FakeCallException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>Integrates the details into the message.</summary>
    /// <param name="data">Associated call data.</param>
    /// <param name="setup">Call data with behavior.</param>
    /// <returns>Message to use for the exception.</returns>
    private static string BuildMessage(CallData data, IEnumerable<CallData> setup)
    {
        string nl = Environment.NewLine;

        return "Method called without matched behavior." +
            ((data == null) ? "" : $"{nl}Call: {data}") +
            ((setup == null) ? "" : $"{nl}Calls with behavior set: {BuildMessage(setup)}");
    }

    /// <summary>Integrates the details into the message.</summary>
    /// <param name="setup">Call data with behavior.</param>
    /// <returns>Message to use for the exception.</returns>
    private static string BuildMessage(IEnumerable<CallData> setup)
    {
        return Environment.NewLine + string.Join(Environment.NewLine, setup);
    }
}
