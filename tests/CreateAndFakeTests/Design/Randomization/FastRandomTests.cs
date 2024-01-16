using System.Linq;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Design.Randomization;

/// <summary>Verifies behavior.</summary>
public sealed class FastRandomTests : ValueRandomTestBase<FastRandom>
{
    /// <summary>Invalid values to test for.</summary>
    private static readonly double[] _BadDoubles = [
        double.NaN,
        double.NegativeInfinity,
        double.PositiveInfinity];

    /// <summary>Invalid values to test for.</summary>
    private static readonly float[] _BadFloats = [
        float.NaN,
        float.NegativeInfinity,
        float.PositiveInfinity];

    [Fact]
    internal static async Task Create_InvalidValuesPossible()
    {
        FastRandom random = new(false);

        Limiter limiter = new(15000);
        await limiter.StallUntil(() =>
            _BadDoubles.Contains(random.Next<double>())).ConfigureAwait(true);
        await limiter.StallUntil(() =>
            _BadFloats.Contains(random.Next<float>())).ConfigureAwait(true);
    }

    [Fact]
    internal static async Task Create_OnlyValidValuesPreventsInvalids()
    {
        FastRandom random = new(true);

        await Limiter.Myriad.Repeat(() => Tools.Asserter.Is(false,
            _BadDoubles.Contains(random.Next<double>()))).ConfigureAwait(true);
        await Limiter.Myriad.Repeat(() => Tools.Asserter.Is(false,
            _BadFloats.Contains(random.Next<float>()))).ConfigureAwait(true);
    }
}
