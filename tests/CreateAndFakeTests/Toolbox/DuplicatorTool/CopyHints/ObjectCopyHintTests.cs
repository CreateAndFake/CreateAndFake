using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Verifies behavior.</summary>
public sealed class ObjectCopyHintTests : CopyHintTestBase<ObjectCopyHint>
{
    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes = [
        typeof(PrivateValuerEquatableSample),
        typeof(IUnimplementedSample),
        typeof(DataHolderSample),
        typeof(FieldSample),
        typeof(object)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(MismatchDataSample)];

    /// <summary>Sets up the tests.</summary>
    public ObjectCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }
}
