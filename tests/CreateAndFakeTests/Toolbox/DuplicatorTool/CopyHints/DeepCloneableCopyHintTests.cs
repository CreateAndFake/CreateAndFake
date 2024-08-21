using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Verifies behavior.</summary>
public sealed class DeepCloneableCopyHintTests : CopyHintTestBase<DeepCloneableCopyHint>
{
    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = Enumerable.Repeat(typeof(IDeepCloneable), 10).ToArray();

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object)];

    /// <summary>Sets up the tests.</summary>
    public DeepCloneableCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }
}
