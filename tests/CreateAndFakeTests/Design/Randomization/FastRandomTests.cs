using System.Linq;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Randomization
{
    /// <summary>Verifies behavior.</summary>
    public sealed class FastRandomTests : ValueRandomTestBase<FastRandom>
    {
        /// <summary>Invalid values to test for.</summary>
        private static readonly double[] badDoubles = new[] {
            double.NaN, double.NegativeInfinity, double.PositiveInfinity };

        /// <summary>Invalid values to test for.</summary>
        private static readonly float[] badFloats = new[] {
            float.NaN, float.NegativeInfinity, float.PositiveInfinity };

        /// <summary>Verifies invalid values are creatable.</summary>
        [Fact]
        public static async Task Create_InvalidValuesPossible()
        {
            IRandom random = new FastRandom(false);

            Limiter limiter = new Limiter(15000);
            await limiter.StallUntil(() => badDoubles.Contains(random.Next<double>()));
            await limiter.StallUntil(() => badFloats.Contains(random.Next<float>()));
        }

        /// <summary>Verifies invalid values can be prevented.</summary>
        [Fact]
        public static async Task Create_OnlyValidValuesPreventsInvalids()
        {
            IRandom random = new FastRandom(true);

            await Limiter.Myriad.Repeat(() => Tools.Asserter.Is(false, badDoubles.Contains(random.Next<double>())));
            await Limiter.Myriad.Repeat(() => Tools.Asserter.Is(false, badFloats.Contains(random.Next<float>())));
        }
    }
}
