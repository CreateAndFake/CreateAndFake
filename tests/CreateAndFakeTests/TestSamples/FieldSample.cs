using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class FieldSample
    {
        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        [SuppressMessage("Sonar", "S1104:EncapsulateFields", Justification = "For testing.")]
        public string StringValue;

        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        [SuppressMessage("Sonar", "S1104:EncapsulateFields", Justification = "For testing.")]
        public int NumberValue;

        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        [SuppressMessage("Sonar", "S1104:EncapsulateFields", Justification = "For testing.")]
        public IEnumerable<string> CollectionValue;

        /// <summary>For testing.</summary>
        public FieldSample(string stringValue)
        {
            StringValue = stringValue;
        }
    }
}
