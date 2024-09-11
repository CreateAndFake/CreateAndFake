﻿using CreateAndFake.Design;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

public sealed class ObjectCreateHintTests : CreateHintTestBase<ObjectCreateHint>
{
    private static readonly ObjectCreateHint _TestInstance = new();

    private static readonly Type[] _ValidTypes =
    [
        typeof(object),
        typeof(Arg),
        typeof(DataHolderSample),
        typeof(IUnimplementedSample),
        typeof(FieldSample),
        typeof(FactorySample)
    ];

    private static readonly Type[] _InvalidTypes = [typeof(VoidType)];

    public ObjectCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Theory, RandomData]
    public void ObjectCreateHint_CanHandleInfinites(InfiniteSample sample1, ParentLoopSample sample2)
    {
        sample1.Assert().IsNot(null);
        sample2.Assert().IsNot(null);
    }

    [Fact]
    public async Task Create_RetriesUntilGoodSample()
    {
        await Limiter.Dozen.Repeat("",
            () => typeof(IIsGoodOrBadSample).CreateRandomInstance().Assert().IsNot(null)).ConfigureAwait(true);
    }
}
