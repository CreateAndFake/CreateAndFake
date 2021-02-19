using System.Linq;
using CreateAndFake;
using CreateAndFake.Design;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue052Tests
    {
        internal class Data
        {
            public string Item { get; set; }
        }

        [Theory, RandomData]
        internal static void Issue052_RandomizerPostCustomizable(Fake<CreateHint> hint)
        {
            IRandomizer randomizer = new Randomizer(Tools.Faker, Tools.Gen, Limiter.Dozen, false);
            Data testItem = new() { Item = "Sample" };

            hint.Setup(
                d => d.TryCreate(typeof(Data), Arg.Any<RandomizerChainer>()),
                Behavior.Returns((true, (object)testItem), Times.Once));

            randomizer.AddHint(hint.Dummy);
            randomizer.Create<Data>().Assert().ReferenceEqual(testItem);
            hint.VerifyAll();
        }

        [Theory, RandomData]
        internal static void Issue052_DuplicatorPostCustomizable(Fake<CopyHint> hint, Data item)
        {
            IDuplicator duplicator = new Duplicator(Tools.Asserter, false);

            hint.Setup(
                d => d.TryCopy(item, Arg.Any<DuplicatorChainer>()),
                Behavior.Returns((true, (object)item), Times.Once));

            duplicator.AddHint(hint.Dummy);
            duplicator.Copy(item).Assert().ReferenceEqual(item);
            hint.VerifyAll();
        }

        [Theory, RandomData]
        internal static void Issue052_ValuerPostCustomizable(Fake<CompareHint> hint, Data item1, Data item2)
        {
            IValuer valuer = new Valuer(false);

            hint.Setup("Supports",
                new object[] { item1, item2, Arg.LambdaAny<ValuerChainer>() },
                Behavior.Returns(true, Times.Once));
            hint.Setup("Compare",
                new object[] { item1, item2, Arg.LambdaAny<ValuerChainer>() },
                Behavior.Returns(Enumerable.Empty<Difference>(), Times.Once));

            valuer.AddHint(hint.Dummy);
            valuer.Equals(item1, item2).Assert().Is(true);
            hint.VerifyAll();
        }
    }
}
