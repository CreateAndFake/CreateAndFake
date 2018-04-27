using System.Diagnostics.CodeAnalysis;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Represents any generic for mock matching.</summary>
    [SuppressMessage("Sonar", "S3453:Uncreatable", Justification = "Intended to be uncreatable.")]
    public sealed class AnyGeneric
    {
        /// <summary>Prevents instantiation.</summary>
        private AnyGeneric() { }
    }
}
