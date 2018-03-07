using System.Collections;
using System.Collections.Generic;

namespace CreateAndFake.Design.Content
{
    /// <summary>Compares objects by value.</summary>
    public sealed class ValueComparer :
        IComparer,
        IComparer<object>,
        IComparer<IValueEquatable>,
        IComparer<IEnumerable>,
        IComparer<IDictionary>,
        IEqualityComparer,
        IEqualityComparer<object>,
        IEqualityComparer<IValueEquatable>,
        IEqualityComparer<IEnumerable>,
        IEqualityComparer<IDictionary>
    {
        /// <summary>Hash used for null values.</summary>
        public static int NullHash { get; } = 0;

        /// <summary>Starting hash value.</summary>
        public static int BaseHash { get; } = 1009;

        /// <summary>Multplier for computing hashes.</summary>
        public static int HashMultiplier { get; } = 92821;

        /// <summary>Default instance for use.</summary>
        public static ValueComparer Use { get; } = new ValueComparer();

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public new bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            else if (x is null || y is null)
            {
                return false;
            }
            else if (x is IValueEquatable asValue)
            {
                return Equals(asValue, y as IValueEquatable);
            }
            else if (x is IEnumerable asEnum)
            {
                return Equals(asEnum, y as IEnumerable);
            }
            else
            {
                return x.Equals(y);
            }
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public bool Equals(IValueEquatable x, IValueEquatable y)
        {
            return x?.ValuesEqual(y) ?? y?.ValuesEqual(x) ?? true;
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public bool Equals(IEnumerable x, IEnumerable y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            else if (x is null || y is null)
            {
                return false;
            }
            else if (x is string)
            {
                return x.Equals(y);
            }
            else if (x is IDictionary asDict)
            {
                return Equals(asDict, y as IDictionary);
            }
            else
            {
                IEnumerator xGen = x.GetEnumerator();
                IEnumerator yGen = y.GetEnumerator();

                while (xGen.MoveNext())
                {
                    if (!yGen.MoveNext() || !Equals(xGen.Current, yGen.Current))
                    {
                        return false;
                    }
                }
                return !yGen.MoveNext();
            }
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>True if equal; false otherwise.</returns>
        public bool Equals(IDictionary x, IDictionary y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            else if (x is null || y is null)
            {
                return false;
            }
            else if (x.Count != y.Count)
            {
                return false;
            }
            else
            {
                foreach (DictionaryEntry entry in x)
                {
                    if (!y.Contains(entry.Key) || !Equals(entry.Value, y[entry.Key]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">Object to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        public int GetHashCode(object obj)
        {
            if (obj is null)
            {
                return NullHash;
            }
            else if (obj is IValueEquatable asValue)
            {
                return GetHashCode(asValue);
            }
            else if (obj is IEnumerable asEnum)
            {
                return GetHashCode(asEnum);
            }
            else
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>Returns a hash code for the specified objects.</summary>
        /// <param name="items">Objects to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        public int GetHashCode(params object[] items)
        {
            return GetHashCode((IEnumerable)items);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">Object to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        public int GetHashCode(IValueEquatable obj)
        {
            return obj?.GetValueHash() ?? NullHash;
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">Object to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        public int GetHashCode(IEnumerable obj)
        {
            if (obj is null)
            {
                return NullHash;
            }
            else if (obj is string)
            {
                return obj.GetHashCode();
            }
            else if (obj is IDictionary asDict)
            {
                return GetHashCode(asDict);
            }
            else
            {
                int hash = BaseHash;
                foreach (object item in obj)
                {
                    hash = hash * HashMultiplier + GetHashCode(item);
                }
                return hash;
            }
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">Object to generate a code for.</param>
        /// <returns>The generated hash.</returns>
        public int GetHashCode(IDictionary obj)
        {
            if (obj is null) return NullHash;

            int hash = BaseHash;
            foreach (DictionaryEntry item in obj)
            {
                hash += GetHashCode(item.Key);
                hash += GetHashCode(item.Value);
            }
            return hash;
        }

        /// <summary>Compares to sort values by their value hash.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Difference between value hashes.</returns>
        public int Compare(object x, object y)
        {
            return ReferenceEquals(x, y) ? 0 : GetHashCode(x) - GetHashCode(y);
        }

        /// <summary>Compares to sort values by their value hash.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Difference between value hashes.</returns>
        public int Compare(IValueEquatable x, IValueEquatable y)
        {
            return ReferenceEquals(x, y) ? 0 : GetHashCode(x) - GetHashCode(y);
        }

        /// <summary>Compares to sort values by their value hash.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Difference between value hashes.</returns>
        public int Compare(IEnumerable x, IEnumerable y)
        {
            return ReferenceEquals(x, y) ? 0 : GetHashCode(x) - GetHashCode(y);
        }

        /// <summary>Compares to sort values by their value hash.</summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Difference between value hashes.</returns>
        public int Compare(IDictionary x, IDictionary y)
        {
            return ReferenceEquals(x, y) ? 0 : GetHashCode(x) - GetHashCode(y);
        }
    }
}
