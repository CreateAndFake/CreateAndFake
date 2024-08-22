using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFakeTests.Toolbox.AsserterTool.Fluent;

public static class AssertTextTests
{
    [Fact]
    internal static void AssertText_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<AssertText>();
    }

    [Fact]
    internal static void AssertText_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation<AssertText>();
    }

    [Theory, RandomData]
    internal static void Contains_UsingSubstring_Success(ICollection<string> sample)
    {
        string.Join("", sample).Assert().Contains(Tools.Gen.NextItem(sample));
    }

    [Theory, RandomData]
    internal static void Contains_RandomOther_Failure(string sample)
    {
        sample.Assert(d => d.Assert().Contains(Tools.Mutator.Variant(sample))).Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void ContainsNot_RandomOther_Success(string sample)
    {
        sample.Assert().ContainsNot(Tools.Mutator.Variant(sample));
    }

    [Theory, RandomData]
    internal static void ContainsNot_UsingSubstring_Failure(ICollection<string> sample)
    {
        string.Join("", sample)
            .Assert(d => d.Assert().ContainsNot(Tools.Gen.NextItem(sample)))
            .Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void StartsWith_UsingFirstString_Success(ICollection<string> sample)
    {
        string.Join("", sample).Assert().StartsWith(sample.First());
    }

    [Theory, RandomData]
    internal static void StartsWith_UsingNonFirstString_Failure([Size(3)] ICollection<string> sample)
    {
        string.Join("", sample)
            .Assert(d => d.Assert().StartsWith(Tools.Gen.NextItem(sample.Skip(1))))
            .Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void StartsNotWith_UsingNonFirstString_Success([Size(3)] ICollection<string> sample)
    {
        string.Join("", sample).Assert().StartsNotWith(Tools.Gen.NextItem(sample.Skip(1)));
    }

    [Theory, RandomData]
    internal static void StartsNotWith_UsingFirstString_Failure(ICollection<string> sample)
    {
        string.Join("", sample)
            .Assert(d => d.Assert().StartsNotWith(sample.First()))
            .Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void EndsWith_UsingLastString_Success(ICollection<string> sample)
    {
        string.Join("", sample).Assert().EndsWith(sample.Last());
    }

    [Theory, RandomData]
    internal static void EndsWith_UsingNonLstString_Failure([Size(3)] ICollection<string> sample)
    {
        string.Join("", sample)
            .Assert(d => d.Assert().EndsWith(Tools.Gen.NextItem(sample.Reverse().Skip(1))))
            .Throws<AssertException>();
    }

    [Theory, RandomData]
    internal static void EndsNotWith_UsingNonLastString_Success([Size(3)] ICollection<string> sample)
    {
        string.Join("", sample).Assert().EndsNotWith(Tools.Gen.NextItem(sample.Reverse().Skip(1)));
    }

    [Theory, RandomData]
    internal static void EndsNotWith_UsingLstString_Failure(ICollection<string> sample)
    {
        string.Join("", sample)
            .Assert(d => d.Assert().EndsNotWith(sample.Last()))
            .Throws<AssertException>();
    }
}
