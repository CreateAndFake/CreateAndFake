using System;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class ArgTests
    {
        /// <summary>Verifies all methods come in pairs with a lambda version.</summary>
        [Fact]
        public static void Arg_PairsWithLambda()
        {
            string[] methods = typeof(Arg)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Select(m => m.Name)
                .ToArray();

            Tools.Asserter.IsEmpty(methods
                .Where(m => !m.StartsWith("Lambda", StringComparison.InvariantCulture))
                .Where(m => !methods.Contains("Lambda" + m)),
                "Methods should have a lambda version to convert to.");
        }

        /// <summary>Verifies regular args just return default.</summary>
        [Fact]
        public static void Arg_Defaults()
        {
            Tools.Asserter.Is(default(int), Arg.Any<int>());
            Tools.Asserter.Is(default(int), Arg.Where<int>(null));

            Tools.Asserter.Is(default(string), Arg.Any<string>());
            Tools.Asserter.Is(default(string), Arg.NotNull<string>());
            Tools.Asserter.Is(default(string), Arg.Where<string>(null));
        }

        /// <summary>Verifies args for references aren't just default.</summary>
        [Fact]
        public static void Arg_OutRefDefault()
        {
            Tools.Asserter.Is(default(int), Arg.AnyRef<int>().Var);
            Tools.Asserter.Is(default(int), Arg.WhereRef<int>(null).Var);

            Tools.Asserter.Is(default(string), Arg.AnyRef<string>().Var);
            Tools.Asserter.Is(default(string), Arg.WhereRef<string>(null).Var);
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [Fact]
        public static void LambdaAny_MatchesValues()
        {
            Tools.Asserter.Is(true, Arg.LambdaAny<int>().Matches(Tools.Randomizer.Create<int>()));
            Tools.Asserter.Is(true, Arg.LambdaAny<string>().Matches(Tools.Randomizer.Create<string>()));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [Fact]
        public static void LambdaAny_NullsMatch()
        {
            Tools.Asserter.Is(true, Arg.LambdaAny<string>().Matches(null));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [Fact]
        public static void LambdaAnyRef_MatchesValues()
        {
            Tools.Asserter.Is(true, Arg.LambdaAnyRef<int>().Matches(Tools.Randomizer.Create<OutRef<int>>()));
            Tools.Asserter.Is(true, Arg.LambdaAnyRef<string>().Matches(Tools.Randomizer.Create<OutRef<string>>()));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [Fact]
        public static void LambdaNotNull_MatchesValues()
        {
            Tools.Asserter.Is(true, Arg.LambdaNotNull<string>().Matches(Tools.Randomizer.Create<string>()));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [Fact]
        public static void LambdaNotNull_NullsInvalid()
        {
            Tools.Asserter.Is(false, Arg.LambdaNotNull<string>().Matches(null));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [Fact]
        public static void LambdaWhere_MatchesCondition()
        {
            Tools.Asserter.Is(true, Arg.LambdaWhere<string>(null)
                .Matches(Tools.Randomizer.Create<string>()));
            Tools.Asserter.Is(true, Arg.LambdaWhere<string>(s => true)
                .Matches(Tools.Randomizer.Create<string>()));
            Tools.Asserter.Is(false, Arg.LambdaWhere<string>(s => false)
                .Matches(Tools.Randomizer.Create<string>()));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [Fact]
        public static void LambdaWhereRef_MatchesCondition()
        {
            Tools.Asserter.Is(true, Arg.LambdaWhereRef<string>(null)
                .Matches(Tools.Randomizer.Create<OutRef<string>>()));
            Tools.Asserter.Is(true, Arg.LambdaWhereRef<string>(s => true)
                .Matches(Tools.Randomizer.Create<OutRef<string>>()));
            Tools.Asserter.Is(false, Arg.LambdaWhereRef<string>(s => false)
                .Matches(Tools.Randomizer.Create<OutRef<string>>()));
        }

        /// <summary>Verifies the arg can clone.</summary>
        [Theory, RandomData]
        public static void Arg_Cloneable(string value)
        {
            Arg origin = Arg.LambdaWhere<string>(d => d == value);
            Arg copy = Tools.Duplicator.Copy(origin);

            Tools.Asserter.Is(origin, copy);
            Tools.Asserter.Is(true, copy.Matches(value));
            Tools.Asserter.Is(false, copy.Matches(Tools.Mutator.Variant(value)));
        }
    }
}
