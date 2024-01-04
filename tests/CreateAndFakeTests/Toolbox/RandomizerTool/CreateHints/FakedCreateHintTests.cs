using System;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

/// <summary>Verifies behavior.</summary>
public sealed class FakedCreateHintTests : CreateHintTestBase<FakedCreateHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly FakedCreateHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = [typeof(IFaked)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public FakedCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }
}
