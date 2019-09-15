using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.TestBases
{
    /// <summary>Verifies behavior.</summary>
    public abstract class ValueRandomTestBase<T> where T : ValueRandom
    {
        /// <summary>Instance to test with.</summary>
        private static readonly ValueRandom s_TestInstance = Tools.Randomizer.Create<T>();

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public void ValueRandom_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<T>();
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public void ValueRandom_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<T>();
        }

        /// <summary>Verifies intended value types work.</summary>
        [Fact]
        public void Supports_TypeCoverage()
        {
            Tools.Asserter.Is(true, s_TestInstance.Supports<double>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<ushort>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<ulong>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<float>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<short>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<uint>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<long>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<char>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<int>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<byte>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<sbyte>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<bool>());
            Tools.Asserter.Is(true, s_TestInstance.Supports<decimal>());
        }

        /// <summary>Verifies an invalid type is not supported.</summary>
        [Fact]
        public void Supports_InvalidTypeFalse()
        {
            Tools.Asserter.Is(false, s_TestInstance.Supports(typeof(object)));
        }

        /// <summary>Verifies intended value types work.</summary>
        [Fact]
        public void Next_TypeCoverage()
        {
            s_TestInstance.Next<double>();
            s_TestInstance.Next<ushort>();
            s_TestInstance.Next<ulong>();
            s_TestInstance.Next<float>();
            s_TestInstance.Next<short>();
            s_TestInstance.Next<uint>();
            s_TestInstance.Next<long>();
            s_TestInstance.Next<char>();
            s_TestInstance.Next<int>();
            s_TestInstance.Next<byte>();
            s_TestInstance.Next<sbyte>();
            s_TestInstance.Next<decimal>();
            s_TestInstance.Next<bool>();
        }

        /// <summary>Verifies intended value types work.</summary>
        [Fact]
        public void Next_MaxTypeCoverage()
        {
            s_TestInstance.Next(double.MaxValue);
            s_TestInstance.Next(ushort.MaxValue);
            s_TestInstance.Next(ulong.MaxValue);
            s_TestInstance.Next(float.MaxValue);
            s_TestInstance.Next(short.MaxValue);
            s_TestInstance.Next(uint.MaxValue);
            s_TestInstance.Next(long.MaxValue);
            s_TestInstance.Next(char.MaxValue);
            s_TestInstance.Next(int.MaxValue);
            s_TestInstance.Next(byte.MaxValue);
            s_TestInstance.Next(sbyte.MaxValue);
            s_TestInstance.Next(decimal.MaxValue);
            s_TestInstance.Next(true);
        }

        /// <summary>Verifies intended value types work.</summary>
        [Fact]
        public void Next_MinTypeCoverage()
        {
            s_TestInstance.Next(double.MinValue, double.MaxValue);
            s_TestInstance.Next(ushort.MinValue, ushort.MaxValue);
            s_TestInstance.Next(ulong.MinValue, ulong.MaxValue);
            s_TestInstance.Next(float.MinValue, float.MaxValue);
            s_TestInstance.Next(short.MinValue, short.MaxValue);
            s_TestInstance.Next(uint.MinValue, uint.MaxValue);
            s_TestInstance.Next(long.MinValue, long.MaxValue);
            s_TestInstance.Next(char.MinValue, char.MaxValue);
            s_TestInstance.Next(int.MinValue, int.MaxValue);
            s_TestInstance.Next(byte.MinValue, byte.MaxValue);
            s_TestInstance.Next(sbyte.MinValue, sbyte.MaxValue);
            s_TestInstance.Next(decimal.MinValue, decimal.MaxValue);
            s_TestInstance.Next(false, true);
        }

        /// <summary>Verifies backup stumble behavior works.</summary>
        [Fact]
        public async Task Next_StumbleWorks()
        {
            int min = int.MinValue / 2 - 1;
            int max = int.MaxValue / 2 + 1;

            await Limiter.Myriad.Repeat(() =>
            {
                int result = s_TestInstance.Next(min, max);

                Tools.Asserter.Is(true, result >= min, "Value lower than min was returned.");
                Tools.Asserter.Is(true, result < max, "Value higher than max was returned.");
            });
        }

        /// <summary>Verifies that an invalid type throws.</summary>
        [Fact]
        public void Next_InvalidTypeThrows()
        {
            Tools.Asserter.Throws<NotSupportedException>(
                () => s_TestInstance.Next(typeof(object)));
        }

        /// <summary>Verifies the same min and max can be used.</summary>
        [Fact]
        public void Next_SameMinMaxWorks()
        {
            Tools.Asserter.Is(0, s_TestInstance.Next(0, 0));
        }

        /// <summary>Verifies different values are generated.</summary>
        [Fact]
        public void Next_Variation()
        {
            Tools.Asserter.IsNot(s_TestInstance.Next<long>(), s_TestInstance.Next<long>());
        }

        /// <summary>Verifies unsupported types don't work.</summary>
        [Fact]
        public void Next_UnsupportedTypeThrows()
        {
            Tools.Asserter.Throws<NotSupportedException>(
                () => s_TestInstance.Next<StructSample>());

            Tools.Asserter.Throws<NotSupportedException>(
                () => s_TestInstance.Next(Tools.Randomizer.Create<StructSample>()));
        }

        /// <summary>Verifies max is excluded as a possible result.</summary>
        [Fact]
        public void Next_MaxDoubleExcluded()
        {
            double min = 9.9999999;
            double max = 10;

            for (int i = 0; i < 25000; i++)
            {
                double result = s_TestInstance.Next(min, max);

                Tools.Asserter.Is(true, result >= min, "Value lower than min was returned.");
                Tools.Asserter.Is(true, result < max, "Value higher than max was returned.");
            }
        }

        /// <summary>Verifies max is excluded as a possible result.</summary>
        [Fact]
        public void Next_MaxDecimalExcluded()
        {
            decimal min = 9.9999999M;
            decimal max = 10;

            for (int i = 0; i < 25000; i++)
            {
                decimal result = s_TestInstance.Next(min, max);

                Tools.Asserter.Is(true, result >= min, "Value lower than min was returned.");
                Tools.Asserter.Is(true, result < max, "Value higher than max was returned.");
            }
        }

        /// <summary>Verifies max values greater than min can't be used.</summary>
        [Fact]
        public void Next_MinGreaterMaxThrows()
        {
            Tools.Asserter.Throws<ArgumentOutOfRangeException>(
                () => s_TestInstance.Next(0, -1));
        }

        /// <summary>Verifies collections work.</summary>
        [Theory, RandomData]
        public void NextItem_CollectionsWork(ICollection<string> data)
        {
            Tools.Asserter.Is(true, data.Contains(s_TestInstance.NextItem(data)));
        }

        /// <summary>Verifies linq enumerables work.</summary>
        [Fact]
        public void NextItem_YieldWorks()
        {
            Tools.Asserter.IsNot(null, s_TestInstance.NextItem(CreateEnum(1)));
            Tools.Asserter.IsNot(null, s_TestInstance.NextItem(CreateEnum(2)));
            Tools.Asserter.IsNot(null, s_TestInstance.NextItem(CreateEnum(3)));
        }

        /// <summary>Verifies empty enumerables throw.</summary>
        [Fact]
        public void NextItem_EmptyThrows()
        {
            Tools.Asserter.Throws<ArgumentOutOfRangeException>(
                () => s_TestInstance.NextItem(Array.Empty<object>()));

            Tools.Asserter.Throws<InvalidOperationException>(
                () => s_TestInstance.NextItem(CreateEnum(0)));
        }

        /// <summary>Helper for creating linq enumerables.</summary>
        /// <param name="size">Size of the enumerable to create.</param>
        /// <returns>Enumerable to test with.</returns>
        private static IEnumerable<object> CreateEnum(int size)
        {
            for (int i = 0; i < size; i++)
            {
                yield return new object();
            }
        }
    }
}
