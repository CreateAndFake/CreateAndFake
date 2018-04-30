using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing objects for the valuer.</summary>
    public sealed class ObjectCompareHint : CompareHint
    {
        /// <summary>Flags used to find properties and fields.</summary>
        private readonly BindingFlags m_Scope;

        /// <summary>Sets up what to compare.</summary>
        /// <param name="scope">Flags used to find properties and fields.</param>
        public ObjectCompareHint(BindingFlags scope)
        {
            m_Scope = scope;
        }

        /// <summary>Determines if the objects are supported by the hint.</summary>
        /// <param name="expected">First object under question.</param>
        /// <param name="actual">Second object under question.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>True if the objects can be compared; false otherwise.</returns>
        protected override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));

            Type type = expected.GetType();
            return type.GetProperties(m_Scope).Any(p => p.CanRead)
                || type.GetFields(m_Scope).Any();
        }

        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            Type type = expected.GetType();

            foreach (PropertyInfo property in type.GetProperties(m_Scope).Where(p => p.CanRead))
            {
                foreach (Difference diff in valuer.Compare(property.GetValue(expected), property.GetValue(actual)))
                {
                    yield return new Difference(property, diff);
                }
            }

            foreach (FieldInfo field in expected.GetType().GetFields(m_Scope))
            {
                foreach (Difference diff in valuer.Compare(field.GetValue(expected), field.GetValue(actual)))
                {
                    yield return new Difference(field, diff);
                }
            }
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(object item, ValuerChainer valuer)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            Type type = item.GetType();
            int hash = ValueComparer.BaseHash + type.GetHashCode();

            foreach (PropertyInfo property in type.GetProperties(m_Scope).Where(p => p.CanRead))
            {
                hash = hash * ValueComparer.HashMultiplier + valuer.GetHashCode(property.GetValue(item));
            }

            foreach (FieldInfo field in type.GetFields(m_Scope))
            {
                hash = hash * ValueComparer.HashMultiplier + valuer.GetHashCode(field.GetValue(item));
            }

            return hash;
        }
    }
}
