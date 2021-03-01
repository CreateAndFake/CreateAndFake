using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class FieldSample
    {
        /// <summary>For testing.</summary>
        public const int Perm = 1;

        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public readonly string PermText;

        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public string StringValue;

        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public int NumberValue;

        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        public IEnumerable<string> CollectionValue;

        /// <summary>For testing.</summary>
        public FieldSample(string stringValue, string stringValue2)
        {
            StringValue = stringValue;
            PermText = stringValue2;
        }
    }
}
