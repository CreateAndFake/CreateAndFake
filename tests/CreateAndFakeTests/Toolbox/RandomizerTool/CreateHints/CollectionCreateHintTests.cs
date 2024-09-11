using System.Collections;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

public sealed class CollectionCreateHintTests : CreateHintTestBase<CollectionCreateHint>
{
    private static readonly CollectionCreateHint _TestInstance = new();

    private static readonly Type[] _ItemTypes =
    [
        typeof(string),
        typeof(object),
        typeof(int),
        typeof(double),
        typeof(KeyValuePair<string, int>)
    ];

    private static readonly Type[] _ValidTypes = CollectionCreateHint.PotentialCollections
        .Concat([
            typeof(IEnumerable<>),
            typeof(IList<>),
            typeof(ISet<>),
            typeof(IDictionary<,>),
            typeof(IReadOnlyCollection<>),
            typeof(IReadOnlyList<>),
            typeof(IReadOnlyDictionary<,>),
            typeof(int[]),
            typeof(string[]),
            typeof(object[])])
        .Select(MakeDefined)
        .ToArray();

    private static readonly Type[] _InvalidTypes =
    [
        typeof(object),
        typeof(IEnumerable),
        typeof(IEnumerable<>)
    ];

    public CollectionCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    private static Type MakeDefined(Type type)
    {
        if (type.IsGenericTypeDefinition)
        {
            FastRandom random = new();
            return type.MakeGenericType(type.GetGenericArguments().Select(
                t => random.NextItem(_ItemTypes)).ToArray());
        }
        else
        {
            return type;
        }
    }
}
