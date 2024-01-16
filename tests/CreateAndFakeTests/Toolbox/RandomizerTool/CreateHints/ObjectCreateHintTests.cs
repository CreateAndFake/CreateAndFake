using System;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

/// <summary>Verifies behavior.</summary>
public sealed class ObjectCreateHintTests : CreateHintTestBase<ObjectCreateHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly ObjectCreateHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes =
    [
        typeof(object),
        typeof(Arg),
        typeof(DataHolderSample),
        typeof(IUnimplementedSample),
        typeof(FieldSample),
        typeof(FactorySample)
    ];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(VoidType)];

    /// <summary>Sets up the tests.</summary>
    public ObjectCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    /// <summary>Verifies that looping objects can be created.</summary>
    [Theory, RandomData]
    public void ObjectCreateHint_CanHandleInfinites(InfiniteSample sample1, ParentLoopSample sample2)
    {
        sample1.Assert().IsNot(null);
        sample2.Assert().IsNot(null);
    }
}
