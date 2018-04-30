using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandifferTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake
{
    /// <summary>Holds basic implementations of all reflection tools.</summary>
    public static class Tools
    {
        /// <summary>Core value random handler.</summary>
        public static IRandom Gen { get; } = new FastRandom();

        /// <summary>Compares objects by value.</summary>
        public static IValuer Valuer { get; } = new Valuer();

        /// <summary>Creates fake objects.</summary>
        public static IFaker Faker { get; } = new Faker(Valuer);

        /// <summary>Creates objects and populates them with random values.</summary>
        public static IRandomizer Randomizer { get; } = new Randomizer(Faker, Gen);

        /// <summary>Creates random variants of objects.</summary>
        public static IRandiffer Randiffer { get; } = new Randiffer(Randomizer, Valuer, Limiter.Dozen);

        /// <summary>Handles common test scenarios.</summary>
        public static Asserter Asserter { get; } = new Asserter(Valuer);

        /// <summary>Deep clones objects.</summary>
        public static IDuplicator Duplicator { get; } = new Duplicator(Asserter);

        /// <summary>Automates common tests.</summary>
        public static Tester Tester { get; } = new Tester(Gen, Randomizer, Asserter);
    }
}
