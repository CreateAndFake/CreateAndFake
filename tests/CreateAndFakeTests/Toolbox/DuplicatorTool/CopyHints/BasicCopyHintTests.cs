using System.Reflection;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

public sealed class BasicCopyHintTests : CopyHintTestBase<BasicCopyHint>
{
    private static readonly Type[] _ValidTypes =
    [
        typeof(BindingFlags),
        typeof(string),
        typeof(int),
        typeof(decimal)
    ];

    private static readonly Type[] _InvalidTypes = [typeof(DataHolderSample)];

    public BasicCopyHintTests() : base(_ValidTypes, _InvalidTypes, true) { }

    [Fact]
    internal static void TryCopy_HandlesBaseObject()
    {
        object data = new();
        (bool, object) result = new BasicCopyHint().TryCopy(data, CreateChainer());

        result.Assert().Is((true, data));
        result.Item2.Assert().ReferenceEqual(data);
    }
}
