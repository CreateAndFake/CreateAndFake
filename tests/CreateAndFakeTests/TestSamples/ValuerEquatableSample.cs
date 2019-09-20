using System;
using System.Diagnostics.CodeAnalysis;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class ValuerEquatableSample : IValuerEquatable
    {
        /// <summary>For testing.</summary>
        public string StringValue { get; set; }

        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "For testing.")]
        [SuppressMessage("Sonar", "S1104:EncapsulateFields", Justification = "For testing.")]
        public int NumberValue;

        /// <summary>Compares by value.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public virtual bool ValuesEqual(object other, IValuer valuer)
        {
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            if (other is ValuerEquatableSample sample)
            {
                return valuer.Equals(StringValue, sample.StringValue)
                    && valuer.Equals(NumberValue, sample.NumberValue);
            }
            else
            {
                return false;
            }
        }

        /// <summary>Generates a hash based upon value.</summary>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash code.</returns>
        public virtual int GetValueHash(IValuer valuer)
        {
            return valuer?.GetHashCode(StringValue, NumberValue) ?? throw new ArgumentNullException(nameof(valuer));
        }
    }
}
