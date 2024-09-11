using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

public sealed class StringCreateHintTests : CreateHintTestBase<StringCreateHint>
{
    private static readonly StringCreateHint _TestInstance = new();

    private static readonly Type[] _ValidTypes = [typeof(string)];

    private static readonly Type[] _InvalidTypes = [typeof(object)];

    public StringCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Fact]
    internal static void TryCreate_SizeConstraintsWork()
    {
        int minSize = 2;
        int range = 3;
        StringCreateHint hint = new(minSize, range, []);

        for (int i = 0; i < 1000; i++)
        {
            string result = (string)hint.TryCreate(typeof(string), CreateChainer()).Item2;

            result.Length.Assert().GreaterThanOrEqualTo(minSize, "Result was too small.");
            result.Length.Assert().LessThan(minSize + range, "Result was too big.");
        }
    }

    [Fact]
    internal static void TryCreate_UsesCharSet()
    {
        StringCreateHint hint = new(3, 0, "a");
        object value = "aaa";
        for (int i = 0; i < 100; i++)
        {
            hint.TryCreate(typeof(string), CreateChainer()).Assert().Is((true, value));
        }

        StringCreateHint hint2 = new(3, 0, "ab");
        for (int i = 0; i < 100; i++)
        {
            (bool, object) result = hint2.TryCreate(typeof(string), CreateChainer());

            result.Item1.Assert().Is(true);
            ((string)result.Item2).Trim('a', 'b').Length.Assert().Is(0);
        }
    }
}
