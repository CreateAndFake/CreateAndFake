using System;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool.Proxy
{
    /// <summary>Verifies behavior.</summary>
    public static class CallDataTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void CallData_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<CallData>();
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void CallData_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<CallData>();
        }

        /// <summary>Verifies no match with different method names.</summary>
        [Theory, RandomData]
        public static void MatchesCall_MethodNameMismatch(DataHolderSample[] data, Type[] generics, string name1)
        {
            string name2 = Tools.Mutator.Variant(name1);

            Tools.Asserter.Is(false, new CallData(name1, generics, data, Tools.Valuer)
                .MatchesCall(new CallData(name2, generics, data, null)));
        }

        /// <summary>Verifies no match with different method names.</summary>
        [Theory, RandomData]
        public static void MatchesCall_GenericsMismatch(DataHolderSample[] data, string name)
        {
            Type[] generics1 = Tools.Randomizer.Create<Type[]>().Except(new[] { typeof(AnyGeneric) }).ToArray();
            Type[] generics2 = Tools.Mutator.Variant(generics1);

            Tools.Asserter.Is(false, new CallData(name, generics1, data, Tools.Valuer)
                .MatchesCall(new CallData(name, generics2, data, null)));
        }

        /// <summary>Verifies always a match when using AnyGeneric.</summary>
        [Theory, RandomData]
        public static void MatchesCall_AnyGenericsMatch(DataHolderSample[] data, string name, Type[] generics1)
        {
            Type[] generics2 = generics1.Select(t => typeof(AnyGeneric)).ToArray();

            Tools.Asserter.Is(true, new CallData(name, generics2, data, Tools.Valuer)
                .MatchesCall(new CallData(name, generics1, data, null)));
        }

        /// <summary>Verifies match functionality.</summary>
        [Theory, RandomData]
        public static void MatchesCall_DataMatchBehavior(string name, Type[] generics, DataHolderSample[] data1)
        {
            DataHolderSample[] data2 = data1.Select(d => Tools.Duplicator.Copy(d)).ToArray();

            Tools.Asserter.Is(true, new CallData(name, generics, data1, Tools.Valuer)
                .MatchesCall(new CallData(name, generics, data2, null)));

            Tools.Asserter.Is(false, new CallData(name, generics, data1, null)
                .MatchesCall(new CallData(name, generics, data2, null)));
        }
    }
}
