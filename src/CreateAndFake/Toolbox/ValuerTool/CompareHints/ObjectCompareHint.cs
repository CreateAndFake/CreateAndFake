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
        private readonly BindingFlags _scope;

        /// <summary>Initializes a new instance of the <see cref="ObjectCompareHint"/> class.</summary>
        /// <param name="scope">Flags used to find properties and fields.</param>
        public ObjectCompareHint(BindingFlags scope)
        {
            _scope = scope;
        }

        /// <inheritdoc/>
        protected override bool Supports(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));

            Type type = expected.GetType();
            return type.GetProperties(_scope).Any(p => p.CanRead)
                || type.GetFields(_scope).Any();
        }

        /// <inheritdoc/>
        protected override IEnumerable<Difference> Compare(object expected, object actual, ValuerChainer valuer)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (actual == null) throw new ArgumentNullException(nameof(actual));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return LazyCompare(expected, actual, valuer);
        }

        /// <inheritdoc cref="Compare"/>
        private IEnumerable<Difference> LazyCompare(object expected, object actual, ValuerChainer valuer)
        {
            Type type = expected.GetType();

            foreach (PropertyInfo property in type.GetProperties(_scope).Where(p => p.CanRead))
            {
                foreach (Difference diff in valuer.Compare(property.GetValue(expected), property.GetValue(actual)))
                {
                    yield return new Difference(property, diff);
                }
            }

            foreach (FieldInfo field in expected.GetType().GetFields(_scope))
            {
                foreach (Difference diff in valuer.Compare(field.GetValue(expected), field.GetValue(actual)))
                {
                    yield return new Difference(field, diff);
                }
            }
        }

        /// <inheritdoc/>
        protected override int GetHashCode(object item, ValuerChainer valuer)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            Type type = item.GetType();
            int hash = ValueComparer.BaseHash + type.GetHashCode();

            foreach (PropertyInfo property in type.GetProperties(_scope).Where(p => p.CanRead))
            {
                hash = hash * ValueComparer.HashMultiplier + valuer.GetHashCode(property.GetValue(item));
            }

            foreach (FieldInfo field in type.GetFields(_scope))
            {
                hash = hash * ValueComparer.HashMultiplier + valuer.GetHashCode(field.GetValue(item));
            }

            return hash;
        }
    }
}
