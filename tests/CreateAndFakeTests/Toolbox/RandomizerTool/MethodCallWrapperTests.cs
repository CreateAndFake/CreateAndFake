﻿using System.Reflection;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.RandomizerTool;

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
        Tools.Randomizer.Create<MethodCallWrapper>().Assert().IsNot(null);
    }

    [Theory, RandomData]
    internal static void MethodCallWrapper_WorksInFake(
        [Stub] IRandomizer rand, MethodCallWrapper wrapper, MethodBase method)
    {
        rand.CreateFor(Arg.Any<MethodBase>(), Arg.Any<object[]>()).SetupReturn(wrapper);
        rand.CreateFor(method).Assert().Is(wrapper);
    }

    [Theory, RandomData]
    internal static void ModifyArg_ThrowsWithUnknownParameter(MethodCallWrapper method, string parameter, object value)
    {
        method.Assert(m => m.ModifyArg(parameter, value)).Throws<KeyNotFoundException>();
    }

    [Theory, RandomData]
    internal static void ModifyArg_CanMutate(DataSample sample)
    {
        MethodCallWrapper wrapper = Tools.Randomizer.CreateFor(
            typeof(DataHolderSample).GetMethod(nameof(DataHolderSample.HasNested)));

        wrapper.ModifyArg("value", sample);
        wrapper.Args.Assert().Contains(sample);
    }

    [Theory, RandomData]
    internal static void InvokeOn_UsesArgs(DataSample sample)
    {
        MethodCallWrapper wrapper = Tools.Randomizer.CreateFor(
            typeof(DataSample).GetMethod(nameof(Equals)));

        wrapper.InvokeOn(sample).Assert().Is(false);
        wrapper.ModifyArg("obj", sample);
        wrapper.InvokeOn(sample).Assert().Is(true);
    }
}
