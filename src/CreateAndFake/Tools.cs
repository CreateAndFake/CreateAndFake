using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.MutatorTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake
{
    /// <summary>Manages implementations of all reflection tools.</summary>
    public static class Tools
    {
        /// <summary>Manages currently used tools.</summary>
        /// <remarks>Should only be modified in module initializer once.</remarks>
        public static ToolSet Source { get; set; } = ToolSet.DefaultSet;

        /// <summary>Core value random handler.</summary>
        public static IRandom Gen => Source.Gen;

        /// <summary>Compares objects by value.</summary>
        public static IValuer Valuer => Source.Valuer;

        /// <summary>Creates fake objects.</summary>
        public static IFaker Faker => Source.Faker;

        /// <summary>Creates objects and populates them with random values.</summary>
        public static IRandomizer Randomizer => Source.Randomizer;

        /// <summary>Changes the value of objects or creates alternatives.</summary>
        public static IMutator Mutator => Source.Mutator;

        /// <summary>Handles common test scenarios.</summary>
        public static Asserter Asserter => Source.Asserter;

        /// <summary>Deep clones objects.</summary>
        public static IDuplicator Duplicator => Source.Duplicator;

        /// <summary>Automates common tests.</summary>
        public static Tester Tester => Source.Tester;
    }
}
