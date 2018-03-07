using System.Diagnostics.CodeAnalysis;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Represents void type for behaviors.</summary>
    [SuppressMessage("Sonar", "S3453:Uncreatable", Justification = "Represents void in generics.")]
    public sealed class VoidType
    {
        /// <summary>Prevents instantiation.</summary>
        private VoidType() { }
    }
}
