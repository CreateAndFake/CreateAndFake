using System;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.MutatorTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake;

/// <summary>Holds implementations of all reflection tools.</summary>
public sealed record ToolSet
{
    /// <summary>Default tools to use.</summary>
    public static ToolSet DefaultSet { get; } = CreateViaSeed(Environment.TickCount);

    /// <summary>Sets up the tools with the given seed.</summary>
    /// <param name="seed">Initial seed for randomization.</param>
    public static ToolSet CreateViaSeed(int seed)
    {
        IRandom gen = new SeededRandom(seed);
        IValuer valuer = new Valuer();
        IFaker faker = new Faker(valuer);
        IRandomizer randomizer = new Randomizer(faker, gen, Limiter.Dozen);
        IMutator mutator = new Mutator(randomizer, valuer, Limiter.Dozen);
        Asserter asserter = new(gen, valuer);
        IDuplicator duplicator = new Duplicator(asserter);
        Tester tester = new(gen, randomizer, duplicator, asserter);

        return new ToolSet
        {
            Gen = gen,
            Valuer = valuer,
            Faker = faker,
            Randomizer = randomizer,
            Mutator = mutator,
            Asserter = asserter,
            Duplicator = duplicator,
            Tester = tester
        };
    }

    /// <inheritdoc cref='IRandom'/>
    public IRandom Gen { get; init; }

    /// <inheritdoc cref='IValuer'/>
    public IValuer Valuer { get; init; }

    /// <inheritdoc cref='IFaker'/>
    public IFaker Faker { get; init; }

    /// <inheritdoc cref='IRandomizer'/>
    public IRandomizer Randomizer { get; init; }

    /// <inheritdoc cref='IMutator'/>
    public IMutator Mutator { get; init; }

    /// <inheritdoc cref='Toolbox.AsserterTool.Asserter'/>
    public Asserter Asserter { get; init; }

    /// <inheritdoc cref='IDuplicator'/>
    public IDuplicator Duplicator { get; init; }

    /// <inheritdoc cref='Toolbox.TesterTool.Tester'/>
    public Tester Tester { get; init; }
}
