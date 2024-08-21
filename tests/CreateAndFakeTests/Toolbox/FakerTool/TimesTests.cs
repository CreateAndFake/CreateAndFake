using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFakeTests.Toolbox.FakerTool;

/// <summary>Verifies behavior.</summary>
public static class TimesTests
{
    [Fact]
    internal static void Times_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<Times>();
    }

    [Fact]
    internal static void Times_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<Times>();
    }

    [Fact]
    internal static void Never_Works()
    {
        Tools.Asserter.Is(true, Times.Never.IsInRange(0));
        Tools.Asserter.Is(false, Times.Never.IsInRange(1));
        Tools.Asserter.Is(false, Times.Never.IsInRange(2));
    }

    [Fact]
    internal static void Once_Works()
    {
        Tools.Asserter.Is(false, Times.Once.IsInRange(0));
        Tools.Asserter.Is(true, Times.Once.IsInRange(1));
        Tools.Asserter.Is(false, Times.Once.IsInRange(2));
    }

    [Theory, RandomData]
    internal static void Exactly_Works(int value)
    {
        Tools.Asserter.Is(false, Times.Exactly(value).IsInRange(value - 1));
        Tools.Asserter.Is(true, Times.Exactly(value).IsInRange(value));
        Tools.Asserter.Is(false, Times.Exactly(value).IsInRange(value + 1));
    }

    [Fact]
    internal static void Between_Works()
    {
        int min, max;
        do
        {
            min = Tools.Randomizer.Create<int>();
            max = Tools.Randomizer.Create<int>();
        } while (min >= max);

        Tools.Asserter.Is(false, Times.Between(min, max).IsInRange(min - 1));
        Tools.Asserter.Is(true, Times.Between(min, max).IsInRange(min));
        Tools.Asserter.Is(true, Times.Between(min, max).IsInRange(min + 1));

        Tools.Asserter.Is(true, Times.Between(min, max).IsInRange(max - 1));
        Tools.Asserter.Is(true, Times.Between(min, max).IsInRange(max));
        Tools.Asserter.Is(false, Times.Between(min, max).IsInRange(max + 1));
    }

    [Theory, RandomData]
    internal static void Min_Works(int value)
    {
        Tools.Asserter.Is(false, Times.Min(value).IsInRange(value - 1));
        Tools.Asserter.Is(true, Times.Min(value).IsInRange(value));
        Tools.Asserter.Is(true, Times.Min(value).IsInRange(value + 1));
    }

    [Fact]
    internal static void Max_Works()
    {
        int value;
        do
        {
            value = Tools.Randomizer.Create<int>();
        } while (value <= 0);

        Tools.Asserter.Is(true, Times.Max(value).IsInRange(value - 1));
        Tools.Asserter.Is(true, Times.Max(value).IsInRange(value));
        Tools.Asserter.Is(false, Times.Max(value).IsInRange(value + 1));
    }

    [Fact]
    internal static void ToString_Terse()
    {
        Tools.Asserter.Is("0", Times.Never.ToString());
        Tools.Asserter.Is("1", Times.Once.ToString());
        Tools.Asserter.Is("2", Times.Exactly(2).ToString());
        Tools.Asserter.Is("[0-1]", Times.Max(1).ToString());
        Tools.Asserter.Is("[1-*]", Times.Min(1).ToString());
        Tools.Asserter.Is("[1-2]", Times.Between(1, 2).ToString());
    }

    [Fact]
    internal static void Equality_MatchesValue()
    {
        int min, max;
        do
        {
            min = Tools.Randomizer.Create<int>();
            max = Tools.Randomizer.Create<int>();
        } while (min >= max);

        Tools.Asserter.Is(false, Times.Once.Equals(null));
        Tools.Asserter.Is(true, Times.Exactly(min).Equals(Times.Between(min, min)));
        Tools.Asserter.Is(true, Times.Exactly(max).Equals((object)Times.Between(max, max)));
        Tools.Asserter.Is(false, Times.Exactly(min).Equals(Times.Between(min, max)));
        Tools.Asserter.Is(false, Times.Exactly(max).Equals((object)Times.Between(min, max)));

        Tools.Asserter.Is(Times.Exactly(min).GetHashCode(), Times.Between(min, min).GetHashCode());
        Tools.Asserter.Is(Times.Exactly(max).GetHashCode(), Times.Between(max, max).GetHashCode());
        Tools.Asserter.IsNot(Times.Exactly(min).GetHashCode(), Times.Between(min, max).GetHashCode());
        Tools.Asserter.IsNot(Times.Exactly(max).GetHashCode(), Times.Between(min, max).GetHashCode());
    }
}
