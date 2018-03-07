using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.RandomizerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class RandomizerChainerTests
    {
        private static readonly Func<Type, IEnumerable<Type>, object> s_DummyCallback = (t, h) => null;

        /// <summary>Verifies bad nulls are prevented.</summary>
        [TestMethod]
        public void New_InvalidNullsThrow()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new RandomizerChainer(Tools.Faker, null, Enumerable.Empty<Type>(), s_DummyCallback));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new RandomizerChainer(Tools.Faker, new FastRandom(), Enumerable.Empty<Type>(), null));
        }

        /// <summary>Verifies a null history is acceptable.</summary>
        [TestMethod]
        public void New_NullHistoryValid()
        {
            Tools.Asserter.IsNot(null, new RandomizerChainer(Tools.Faker, new FastRandom(), null, s_DummyCallback));
        }

        /// <summary>Verifies true when given a created type.</summary>
        [TestMethod]
        public void AlreadyCreated_InHistoryTrue()
        {
            Tools.Asserter.Is(true, new RandomizerChainer(Tools.Faker, new FastRandom(),
                new[] { typeof(string) }, s_DummyCallback).AlreadyCreated<string>());
        }

        /// <summary>Verifies false when given an uncreated type.</summary>
        [TestMethod]
        public void AlreadyCreated_NotInHistoryFalse()
        {
            Tools.Asserter.Is(false, new RandomizerChainer(Tools.Faker, new FastRandom(),
                new[] { typeof(object) }, s_DummyCallback).AlreadyCreated<string>());
        }
    }
}
