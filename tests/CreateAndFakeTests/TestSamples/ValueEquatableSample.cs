using System.Diagnostics.CodeAnalysis;
using CreateAndFake.Design.Content;

namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class ValueEquatableSample : IValueEquatable
    {
        /// <summary>For testing.</summary>
        public string StringValue { get; set; }

        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        [SuppressMessage("Sonar", "S1104:EncapsulateFields", Justification = "For testing.")]
        public int NumberValue;

        /// <summary>Compares by value.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public virtual bool ValuesEqual(object other)
        {
            if (other is ValueEquatableSample sample)
            {
                return StringValue == sample.StringValue
                    && NumberValue == sample.NumberValue;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Generates a hash based upon value.</summary>
        /// <returns>The generated hash code.</returns>
        public virtual int GetValueHash()
        {
            return ValueComparer.Use.GetHashCode(StringValue, NumberValue);
        }
    }
}
