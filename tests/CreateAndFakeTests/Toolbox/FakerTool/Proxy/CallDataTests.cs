using System;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.FakerTool.Proxy
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class CallDataTests
    {
        /// <summary>Verifies nulls are not accepted.</summary>
        [TestMethod]
        public void New_NullsThrow()
        {
            Tools.Asserter.Throws<ArgumentNullException>(() => new CallData(
                null, Tools.Randomizer.Create<Type[]>(), Tools.Randomizer.Create<DataHolderSample[]>(), Tools.Valuer));
            Tools.Asserter.Throws<ArgumentNullException>(() => new CallData(
                Tools.Randomizer.Create<string>(), null, Tools.Randomizer.Create<DataHolderSample[]>(), Tools.Valuer));
            Tools.Asserter.Throws<ArgumentNullException>(() => new CallData(
                Tools.Randomizer.Create<string>(), Tools.Randomizer.Create<Type[]>(), null, Tools.Valuer));
        }

        /// <summary>Verifies a null isn't accepted.</summary>
        [TestMethod]
        public void DeepClone_NullThrows()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => Tools.Randomizer.Create<CallData>().DeepClone(null));
        }

        /// <summary>Verifies a null isn't accepted.</summary>
        [TestMethod]
        public void MatchesCall_NullThrows()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => Tools.Randomizer.Create<CallData>().MatchesCall(null));
        }

        /// <summary>Verifies no match with different method names.</summary>
        [TestMethod]
        public void MatchesCall_MethodNameMismatch()
        {
            DataHolderSample[] data = Tools.Randomizer.Create<DataHolderSample[]>();
            Type[] generics = Tools.Randomizer.Create<Type[]>();
            string name1 = Tools.Randomizer.Create<string>();
            string name2 = Tools.Randiffer.Branch(name1);

            Tools.Asserter.Is(false, new CallData(name1, generics, data, Tools.Valuer)
                .MatchesCall(new CallData(name2, generics, data, null)));
        }

        /// <summary>Verifies no match with different method names.</summary>
        [TestMethod]
        public void MatchesCall_GenericsMismatch()
        {
            DataHolderSample[] data = Tools.Randomizer.Create<DataHolderSample[]>();
            string name = Tools.Randomizer.Create<string>();
            Type[] generics1 = Tools.Randomizer.Create<Type[]>();
            Type[] generics2 = Tools.Randiffer.Branch(generics1);

            Tools.Asserter.Is(false, new CallData(name, generics1, data, Tools.Valuer)
                .MatchesCall(new CallData(name, generics2, data, null)));
        }

        /// <summary>Verifies match functionality.</summary>
        [TestMethod]
        public void MatchesCall_DataMatchBehavior()
        {
            string name = Tools.Randomizer.Create<string>();
            Type[] generics = Tools.Randomizer.Create<Type[]>();
            DataHolderSample[] data1 = Tools.Randomizer.Create<DataHolderSample[]>();
            DataHolderSample[] data2 = data1.Select(d => Tools.Duplicator.Copy(d)).ToArray();

            Tools.Asserter.Is(true, new CallData(name, generics, data1, Tools.Valuer)
                .MatchesCall(new CallData(name, generics, data2, null)));

            Tools.Asserter.Is(false, new CallData(name, generics, data1, null)
                .MatchesCall(new CallData(name, generics, data2, null)));
        }
    }
}
