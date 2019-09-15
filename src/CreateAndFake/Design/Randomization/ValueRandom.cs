﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CreateAndFake.Design.Randomization
{
    /// <summary>Provides the core functionality for generic randomization.</summary>
    public abstract class ValueRandom : IRandom
    {
        /// <summary>Supported types and the methods used to generate them.</summary>
        private static readonly IDictionary<Type, Func<ValueRandom, object>> s_Gens
            = new Dictionary<Type, Func<ValueRandom, object>>
            {
                { typeof(double), gen => Create(gen, BitConverter.ToDouble, 8,
                    double.NaN, double.NegativeInfinity, double.PositiveInfinity) },
                { typeof(float), gen => Create(gen, BitConverter.ToSingle, 4,
                    float.NaN, float.NegativeInfinity, float.PositiveInfinity) },

                { typeof(ushort), gen => Create(gen, BitConverter.ToUInt16, 2) },
                { typeof(ulong), gen => Create(gen, BitConverter.ToUInt64, 8) },
                { typeof(short), gen => Create(gen, BitConverter.ToInt16, 2) },
                { typeof(uint), gen => Create(gen, BitConverter.ToUInt32, 4) },
                { typeof(long), gen => Create(gen, BitConverter.ToInt64, 8) },
                { typeof(char), gen => Create(gen, BitConverter.ToChar, 2) },
                { typeof(int), gen => Create(gen, BitConverter.ToInt32, 4) },

                { typeof(byte), gen => gen.NextBytes(1)[0] },
                { typeof(sbyte), gen => (sbyte)gen.Next<byte>() },
                { typeof(bool), gen => (gen.Next<byte>() > byte.MaxValue / 2) },
                { typeof(decimal), gen => new decimal(gen.Next<int>(), gen.Next<int>(),
                    gen.Next<int>(), gen.Next<bool>(), gen.Next<byte>(29)) }
            };

        /// <summary>Creates values.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="gen">Random instance.</param>
        /// <param name="converter">Converter for the type.</param>
        /// <param name="size">Number of bytes in the type.</param>
        /// <param name="invalids">Special invalid values for the type.</param>
        /// <returns>Created value.</returns>
        private static T Create<T>(ValueRandom gen, Func<byte[], int, T> converter, short size, params T[] invalids)
        {
            T value;
            do
            {
                value = converter.Invoke(gen.NextBytes(size), 0);
            } while (gen.OnlyValidValues && invalids.Any(v => v.Equals(value)));
            return value;
        }

        /// <summary>All default value types.</summary>
        public static ICollection<Type> ValueTypes { get; } = s_Gens.Keys;

        /// <summary>Option to prevent generating invalid values.</summary>
        protected bool OnlyValidValues { get; }

        /// <summary>Sets up the randomizer.</summary>
        /// <param name="onlyValidValues">Option to prevent generating invalid values.</param>
        protected ValueRandom(bool onlyValidValues)
        {
            OnlyValidValues = onlyValidValues;
        }

        /// <summary>Generates a byte array filled with random bytes.</summary>
        /// <param name="length">Length of the array to generate.</param>
        /// <returns>The generated byte array.</returns>
        protected abstract byte[] NextBytes(short length);

        /// <summary>Checks if the given type is supported for randomization.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        /// <returns>True if supported; false otherwise.</returns>
        public bool Supports<T>() where T : struct, IComparable, IComparable<T>, IEquatable<T>
        {
            return Supports(typeof(T));
        }

        /// <summary>Checks if the given type is supported for randomization.</summary>
        /// <param name="type">Type to verify.</param>
        /// <returns>True if supported; false otherwise.</returns>
        public bool Supports(Type type)
        {
            return s_Gens.ContainsKey(type);
        }

        /// <summary>Generates a random value of the given value type.</summary>
        /// <typeparam name="T">Value type to generate.</typeparam>
        /// <returns>The generated value.</returns>
        /// <exception cref="NotSupportedException">If the type isn't supported.</exception>
        public T Next<T>() where T : struct, IComparable, IComparable<T>, IEquatable<T>
        {
            return (T)Next(typeof(T));
        }

        /// <summary>Generates a random value of the given value type.</summary>
        /// <param name="type">Value type to generate.</param>
        /// <returns>The generated value.</returns>
        /// <exception cref="NotSupportedException">If the type isn't supported.</exception>
        public object Next(Type type)
        {
            if (!Supports(type))
            {
                throw new NotSupportedException($"Type '{type.Name}' not supported.");
            }
            else
            {
                return s_Gens[type].Invoke(this);
            }
        }

        /// <summary>Generates a positive constrained value of the given value type.</summary>
        /// <typeparam name="T">Value type to generate.</typeparam>
        /// <param name="max">Positive exclusive upper boundary for the value.</param>
        /// <returns>The generated value.</returns>
        /// <exception cref="NotSupportedException">If the type isn't supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If max is less than or equal to 0.</exception>
        public T Next<T>(T max) where T : struct, IComparable, IComparable<T>, IEquatable<T>
        {
            return Next(default, max);
        }

        /// <summary>Generates a constrained value of the given value type.</summary>
        /// <typeparam name="T">Value type to generate.</typeparam>
        /// <param name="min">Inclusive lower boundary for the value.</param>
        /// <param name="max">Exclusive upper boundary for the value.</param>
        /// <returns>The generated value.</returns>
        /// <exception cref="NotSupportedException">If the type isn't supported.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If max is lower than min.</exception>
        public T Next<T>(T min, T max) where T : struct, IComparable, IComparable<T>, IEquatable<T>
        {
            if (!Supports<T>())
            {
                throw new NotSupportedException($"Type '{typeof(T).Name}' not supported.");
            }
            else if (min.Equals(max))
            {
                return min;
            }
            else if (min.CompareTo(max) >= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max,
                    $"Value must be greater than given min: '{min}'.");
            }
            else if (typeof(T) == typeof(bool))
            {
                return default;
            }
            else
            {
                try
                {
                    return CalcNext(min, max);
                }
                catch (ArithmeticException)
                {
                    return StumbleNext(min, max);
                }
            }
        }

        /// <summary>Uses an algorithm to generate the next value.</summary>
        /// <typeparam name="T">Value type to generate.</typeparam>
        /// <param name="min">Inclusive lower boundary for the value.</param>
        /// <param name="max">Exclusive upper boundary for the value.</param>
        /// <returns>The generated value.</returns>
        private T CalcNext<T>(T min, T max) where T : struct, IComparable, IComparable<T>, IEquatable<T>
        {
            dynamic percent;

            // Creates a number in the range of 0.0 to 1.0.
            if (typeof(T) != typeof(decimal))
            {
                percent = (double)Next<uint>() / uint.MaxValue;
            }
            else
            {
                percent = (decimal)Next<uint>() / uint.MaxValue;
            }

            checked
            {
                T value = (T)(percent * ((dynamic)max - min) + min);

                // Prevent any issues steming from imprecision.
                return (value.CompareTo(max) < 0) ? value : min;
            }
        }

        /// <summary>Randoms until acceptable for the next value.</summary>
        /// <typeparam name="T">Value type to generate.</typeparam>
        /// <param name="min">Inclusive lower boundary for the value.</param>
        /// <param name="max">Exclusive upper boundary for the value.</param>
        /// <returns>The generated value.</returns>
        private T StumbleNext<T>(T min, T max) where T : struct, IComparable, IComparable<T>, IEquatable<T>
        {
            dynamic value;
            do
            {
                value = Next<T>();
            } while (value < min || value >= max);
            return value;
        }

        /// <summary>Picks a random item from a collection.</summary>
        /// <typeparam name="T">Type of items being picked.</typeparam>
        /// <param name="items">Collection of items to pick from.</param>
        /// <returns>The picked item.</returns>
        public T NextItem<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            else if (items is ICollection<T> collection)
            {
                return collection.ElementAt(Next(collection.Count));
            }
            else
            {
                return items.OrderBy(i => Next<int>()).First();
            }
        }
    }
}
