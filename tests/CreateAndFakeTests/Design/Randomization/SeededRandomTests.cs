using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Randomization
{
    /// <summary>Verifies behavior.</summary>
    public sealed class SeededRandomTests : ValueRandomTestBase<SeededRandom>
    {
        [Fact]
        internal static void Seed_Deterministic()
        {
            SeededRandom random1 = new SeededRandom();
            SeededRandom random2 = new SeededRandom(random1.Seed);

            Tools.Asserter.Is(random1.Next<int>(), random2.Next<int>());
            Tools.Asserter.Is(random1.Next<int>(), random2.Next<int>());

            random2 = new SeededRandom(random1.Seed);

            Tools.Asserter.Is(random1.Next<int>(), random2.Next<int>());
            Tools.Asserter.Is(random1.Seed, random2.Seed);
        }
    }
}
