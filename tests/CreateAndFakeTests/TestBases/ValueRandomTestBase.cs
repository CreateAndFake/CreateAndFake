using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.TestBases;

/// <summary>Handles testing value random classes.</summary>
/// <typeparam name="T">Value random type to test.</typeparam>
public abstract class ValueRandomTestBase<T> where T : ValueRandom
{
    /// <summary>Instance to test with.</summary>
    private static readonly ValueRandom _TestInstance = Tools.Randomizer.Create<T>();

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
        Tools.Asserter.Is(true, _TestInstance.Supports<double>());
        Tools.Asserter.Is(true, _TestInstance.Supports<ushort>());
        Tools.Asserter.Is(true, _TestInstance.Supports<ulong>());
        Tools.Asserter.Is(true, _TestInstance.Supports<float>());
        Tools.Asserter.Is(true, _TestInstance.Supports<short>());
        Tools.Asserter.Is(true, _TestInstance.Supports<uint>());
        Tools.Asserter.Is(true, _TestInstance.Supports<long>());
        Tools.Asserter.Is(true, _TestInstance.Supports<char>());
        Tools.Asserter.Is(true, _TestInstance.Supports<int>());
        Tools.Asserter.Is(true, _TestInstance.Supports<byte>());
        Tools.Asserter.Is(true, _TestInstance.Supports<sbyte>());
        Tools.Asserter.Is(true, _TestInstance.Supports<bool>());
        Tools.Asserter.Is(true, _TestInstance.Supports<decimal>());
    }

    /// <summary>Verifies an invalid type is not supported.</summary>
    [Fact]
    public void Supports_InvalidTypeFalse()
    {
        Tools.Asserter.Is(false, _TestInstance.Supports(typeof(object)));
    }

    /// <summary>Verifies intended value types work.</summary>
    [Fact]
    public void Next_TypeCoverage()
    {
        _TestInstance.Next<double>();
        _TestInstance.Next<ushort>();
        _TestInstance.Next<ulong>();
        _TestInstance.Next<float>();
        _TestInstance.Next<short>();
        _TestInstance.Next<uint>();
        _TestInstance.Next<long>();
        _TestInstance.Next<char>();
        _TestInstance.Next<int>();
        _TestInstance.Next<byte>();
        _TestInstance.Next<sbyte>();
        _TestInstance.Next<decimal>();
        _TestInstance.Next<bool>();
    }

    /// <summary>Verifies intended value types work.</summary>
    [Fact]
    public void Next_MaxTypeCoverage()
    {
        _TestInstance.Next(double.MaxValue);
        _TestInstance.Next(ushort.MaxValue);
        _TestInstance.Next(ulong.MaxValue);
        _TestInstance.Next(float.MaxValue);
        _TestInstance.Next(short.MaxValue);
        _TestInstance.Next(uint.MaxValue);
        _TestInstance.Next(long.MaxValue);
        _TestInstance.Next(char.MaxValue);
        _TestInstance.Next(int.MaxValue);
        _TestInstance.Next(byte.MaxValue);
        _TestInstance.Next(sbyte.MaxValue);
        _TestInstance.Next(decimal.MaxValue);
        _TestInstance.Next(true);
    }

    /// <summary>Verifies intended value types work.</summary>
    [Fact]
    public void Next_MinTypeCoverage()
    {
        _TestInstance.Next(double.MinValue, double.MaxValue);
        _TestInstance.Next(ushort.MinValue, ushort.MaxValue);
        _TestInstance.Next(ulong.MinValue, ulong.MaxValue);
        _TestInstance.Next(float.MinValue, float.MaxValue);
        _TestInstance.Next(short.MinValue, short.MaxValue);
        _TestInstance.Next(uint.MinValue, uint.MaxValue);
        _TestInstance.Next(long.MinValue, long.MaxValue);
        _TestInstance.Next(char.MinValue, char.MaxValue);
        _TestInstance.Next(int.MinValue, int.MaxValue);
        _TestInstance.Next(byte.MinValue, byte.MaxValue);
        _TestInstance.Next(sbyte.MinValue, sbyte.MaxValue);
        _TestInstance.Next(decimal.MinValue, decimal.MaxValue);
        _TestInstance.Next(false, true);
    }

    /// <summary>Verifies backup stumble behavior works.</summary>
    [Fact]
    public async Task Next_StumbleWorks()
    {
        int min = int.MinValue / 2 - 1;
        int max = int.MaxValue / 2 + 1;

        await Limiter.Myriad.Repeat("", () =>
        {
            int result = _TestInstance.Next(min, max);

            Tools.Asserter.Is(true, result >= min, "Value lower than min was returned.");
            Tools.Asserter.Is(true, result < max, "Value higher than max was returned.");
        }).ConfigureAwait(true);
    }

    /// <summary>Verifies that an invalid type throws.</summary>
    [Fact]
    public void Next_InvalidTypeThrows()
    {
        Tools.Asserter.Throws<NotSupportedException>(
            () => _TestInstance.Next(typeof(object)));
    }

    /// <summary>Verifies the same min and max can be used.</summary>
    [Fact]
    public void Next_SameMinMaxWorks()
    {
        Tools.Asserter.Is(0, _TestInstance.Next(0, 0));
    }

    /// <summary>Verifies different values are generated.</summary>
    [Fact]
    public void Next_Variation()
    {
        Tools.Asserter.IsNot(_TestInstance.Next<long>(), _TestInstance.Next<long>());
    }

    /// <summary>Verifies unsupported types don't work.</summary>
    [Fact]
    public void Next_UnsupportedTypeThrows()
    {
        Tools.Asserter.Throws<NotSupportedException>(
            () => _TestInstance.Next<StructSample>());

        Tools.Asserter.Throws<NotSupportedException>(
            () => _TestInstance.Next(Tools.Randomizer.Create<StructSample>()));
    }

    /// <summary>Verifies max is excluded as a possible result.</summary>
    [Fact]
    public void Next_MaxDoubleExcluded()
    {
        double min = 9.9999999;
        double max = 10;

        for (int i = 0; i < 25000; i++)
        {
            double result = _TestInstance.Next(min, max);

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
            decimal result = _TestInstance.Next(min, max);

            Tools.Asserter.Is(true, result >= min, "Value lower than min was returned.");
            Tools.Asserter.Is(true, result < max, "Value higher than max was returned.");
        }
    }

    /// <summary>Verifies max values greater than min can't be used.</summary>
    [Fact]
    public void Next_MinGreaterMaxThrows()
    {
        Tools.Asserter.Throws<ArgumentOutOfRangeException>(
            () => _TestInstance.Next(0, -1));
    }

    /// <summary>Verifies collections work.</summary>
    [Theory, RandomData]
    public void NextItem_CollectionsWork(ICollection<string> data)
    {
        Tools.Asserter.Is(true, data?.Contains(_TestInstance.NextItem(data)));
    }

    /// <summary>Verifies linq enumerables work.</summary>
    [Fact]
    public void NextItem_YieldWorks()
    {
        Tools.Asserter.IsNot(null, _TestInstance.NextItem(CreateEnum(1)));
        Tools.Asserter.IsNot(null, _TestInstance.NextItem(CreateEnum(2)));
        Tools.Asserter.IsNot(null, _TestInstance.NextItem(CreateEnum(3)));
    }

    /// <summary>Verifies empty enumerables throw.</summary>
    [Fact]
    public void NextItem_EmptyThrows()
    {
        Tools.Asserter.Throws<InvalidOperationException>(
            () => _TestInstance.NextItem(Array.Empty<object>()));

        Tools.Asserter.Throws<InvalidOperationException>(
            () => _TestInstance.NextItem(CreateEnum(0)));
    }

    /// <summary>Verifies empty enumerables give default values.</summary>
    [Fact]
    public void NextItemOrDefault_EmptyGivesDefault()
    {
        Tools.Asserter.Is(0, _TestInstance.NextItemOrDefault((int[])null));
        Tools.Asserter.Is(null, _TestInstance.NextItemOrDefault((object[])null));
        Tools.Asserter.Is(null, _TestInstance.NextItemOrDefault(Array.Empty<object>()));
        Tools.Asserter.Is(null, _TestInstance.NextItemOrDefault(CreateEnum(0)));
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
