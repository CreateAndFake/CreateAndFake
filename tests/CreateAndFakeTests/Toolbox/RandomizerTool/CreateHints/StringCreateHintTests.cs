using System;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class StringCreateHintTests : CreateHintTestBase<StringCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly StringCreateHint _TestInstance = new();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(string) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public StringCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

        [Fact]
        internal static void TryCreate_SizeConstraintsWork()
        {
            int minSize = 2;
            int range = 3;
            StringCreateHint hint = new(minSize, range, Enumerable.Empty<char>());

            for (int i = 0; i < 1000; i++)
            {
                string result = (string)hint.TryCreate(typeof(string), CreateChainer()).Item2;

                Tools.Asserter.Is(true, result.Length >= minSize, "Result was too small.");
                Tools.Asserter.Is(true, result.Length < minSize + range, "Result was too big.");
            }
        }

        [Fact]
        internal static void TryCreate_UsesCharSet()
        {
            StringCreateHint hint = new(3, 0, "a");
            for (int i = 0; i < 100; i++)
            {
                Tools.Asserter.Is((true, "aaa" as object), hint.TryCreate(typeof(string), CreateChainer()));
            }

            StringCreateHint hint2 = new(3, 0, "ab");
            for (int i = 0; i < 100; i++)
            {
                (bool, object) result = hint2.TryCreate(typeof(string), CreateChainer());

                Tools.Asserter.Is(true, result.Item1);
                Tools.Asserter.Is(0, ((string)result.Item2).Trim('a', 'b').Length);
            }
        }
    }
}
