using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

public sealed class SerializableCopyHintTests : CopyHintTestBase<SerializableCopyHint>
{
    private static readonly Type[] _ValidTypes = [typeof(Exception)];

    private static readonly Type[] _InvalidTypes = [typeof(object)];

    public SerializableCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }
}
