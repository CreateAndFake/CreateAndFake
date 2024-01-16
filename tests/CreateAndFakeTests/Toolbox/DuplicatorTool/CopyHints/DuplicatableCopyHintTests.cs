using System;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Verifies behavior.</summary>
public sealed class DuplicatableCopyHintTests : CopyHintTestBase<DuplicatableCopyHint>
{
    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = [typeof(IDuplicatable)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public DuplicatableCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }
}
