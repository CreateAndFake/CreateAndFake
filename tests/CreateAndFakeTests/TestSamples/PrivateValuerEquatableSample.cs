﻿using System;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFakeTests.TestSamples
{
    /// <summary>For testing.</summary>
    public class PrivateValuerEquatableSample : IValuerEquatable
    {
        /// <summary>For testing.</summary>
        private string StringValue { get; }

        /// <summary>For testing.</summary>
        /// <param name="stringValue">For testing.</param>
        public PrivateValuerEquatableSample(string stringValue)
        {
            StringValue = stringValue;
        }

        /// <summary>Compares by value.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public virtual bool ValuesEqual(object other, IValuer valuer)
        {
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            if (other is PrivateValuerEquatableSample sample)
            {
                return valuer.Equals(StringValue, sample.StringValue);
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
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return valuer.GetHashCode(StringValue);
        }
    }
}
