using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.AsserterTool;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue081Tests
    {
        [Fact]
        internal static void Issue081_RandomizationIsSeeded()
        {
            Tools.Gen.InitialSeed.Assert().IsNot(null);
        }

        [Fact]
        internal static void Issue081_AssertionsContainSeed()
        {
            Tools.Asserter.Throws<AssertException>(() => Tools.Asserter.Fail())
                .Message.Contains($"{Tools.Gen.InitialSeed}").Assert().Is(true);
        }
    }
}
