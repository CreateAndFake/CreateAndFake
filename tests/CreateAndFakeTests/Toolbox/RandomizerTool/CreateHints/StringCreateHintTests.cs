using System;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class StringCreateHintTests : CreateHintTestBase<StringCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly StringCreateHint s_TestInstance = new StringCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(string) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public StringCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }

        /// <summary>Verifies the hint creates strings within bounds.</summary>
        [TestMethod]
        public void TryCreate_SizeConstraintsWork()
        {
            int minSize = 2;
            int range = 3;
            StringCreateHint hint = new StringCreateHint(minSize, range, Enumerable.Empty<char>());

            for (int i = 0; i < 1000; i++)
            {
                string result = (string)hint.TryCreate(typeof(string), CreateChainer()).Item2;

                Tools.Asserter.Is(true, result.Length >= minSize, "Result was too small.");
                Tools.Asserter.Is(true, result.Length < minSize + range, "Result was too big.");
            }
        }

        /// <summary>Verifies the hint creates strings with the chars specified.</summary>
        [TestMethod]
        public void TryCreate_UsesCharSet()
        {
            StringCreateHint hint = new StringCreateHint(3, 0, "a");
            for (int i = 0; i < 100; i++)
            {
                Tools.Asserter.Is((true, (object)"aaa"), hint.TryCreate(typeof(string), CreateChainer()));
            }

            StringCreateHint hint2 = new StringCreateHint(3, 0, "ab");
            for (int i = 0; i < 100; i++)
            {
                (bool, object) result = hint2.TryCreate(typeof(string), CreateChainer());

                Tools.Asserter.Is(true, result.Item1);
                Tools.Asserter.Is(0, ((string)result.Item2).Trim('a', 'b').Length);
            }
        }
    }
}
