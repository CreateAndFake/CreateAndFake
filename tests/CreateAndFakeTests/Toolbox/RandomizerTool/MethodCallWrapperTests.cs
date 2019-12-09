using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class MethodCallWrapperTests
    {
        [Fact]
        internal static void MethodCallWrapper_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<MethodCallWrapper>();
        }

        [Fact]
        internal static void MethodCallWrapper_CanRandomize()
        {
            Tools.Asserter.IsNot(null, Tools.Randomizer.Create<MethodCallWrapper>());
        }

        [Theory, RandomData]
        internal static void MethodCallWrapper_WorksInFake(MethodCallWrapper method, Fake<IRandomizer> rand)
        {
            rand.Setup(m => m.CreateFor(Arg.Any<MethodBase>(), Arg.Any<object[]>()), Behavior.Returns(method));

            Tools.Asserter.Is(method, rand.Dummy.CreateFor(Tools.Randomizer.Create<MethodBase>()));
        }

        [Theory, RandomData]
        internal static void ModifyArg_ThrowsWithUnknownParameter(MethodCallWrapper method, string parameter, object value)
        {
            Tools.Asserter.Throws<KeyNotFoundException>(() => method.ModifyArg(parameter, value));
        }

        [Theory, RandomData]
        internal static void ModifyArg_CanMutate(DataSample sample)
        {
            MethodCallWrapper wrapper = Tools.Randomizer.CreateFor(
                typeof(DataHolderSample).GetMethod(nameof(DataHolderSample.HasNested)));

            wrapper.ModifyArg("value", sample);

            Tools.Asserter.Is(true, wrapper.Args.Contains(sample));
        }

        [Theory, RandomData]
        internal static void InvokeOn_UsesArgs(DataSample sample)
        {
            MethodCallWrapper wrapper = Tools.Randomizer.CreateFor(
                typeof(DataSample).GetMethod(nameof(DataSample.Equals)));

            Tools.Asserter.Is(false, wrapper.InvokeOn(sample));
            wrapper.ModifyArg("obj", sample);
            Tools.Asserter.Is(true, wrapper.InvokeOn(sample));
        }
    }
}
