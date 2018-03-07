using System;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class ArgTests
    {
        /// <summary>Verifies all methods come in pairs with a lambda version.</summary>
        [TestMethod]
        public void Arg_PairsWithLambda()
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
        [TestMethod]
        public void Arg_Defaults()
        {
            Tools.Asserter.Is(default(int), Arg.Any<int>());
            Tools.Asserter.Is(default(int), Arg.Where<int>(null));

            Tools.Asserter.Is(default(string), Arg.Any<string>());
            Tools.Asserter.Is(default(string), Arg.NotNull<string>());
            Tools.Asserter.Is(default(string), Arg.Where<string>(null));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [TestMethod]
        public void LambdaAny_MatchesValues()
        {
            Tools.Asserter.Is(true, Arg.LambdaAny<string>().Matches(Tools.Randomizer.Create<string>()));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [TestMethod]
        public void LambdaAny_NullsMatch()
        {
            Tools.Asserter.Is(true, Arg.LambdaAny<string>().Matches(null));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [TestMethod]
        public void LambdaNotNull_MatchesValues()
        {
            Tools.Asserter.Is(true, Arg.LambdaNotNull<string>().Matches(Tools.Randomizer.Create<string>()));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [TestMethod]
        public void LambdaNotNull_NullsInvalid()
        {
            Tools.Asserter.Is(false, Arg.LambdaNotNull<string>().Matches(null));
        }

        /// <summary>Verifies the arg match behavior.</summary>
        [TestMethod]
        public void LambdaWhere_MatchesCondition()
        {
            Tools.Asserter.Is(true, Arg.LambdaWhere<string>(null).Matches(Tools.Randomizer.Create<string>()));
            Tools.Asserter.Is(true, Arg.LambdaWhere<string>(s => true).Matches(Tools.Randomizer.Create<string>()));
            Tools.Asserter.Is(false, Arg.LambdaWhere<string>(s => false).Matches(Tools.Randomizer.Create<string>()));
        }

        /// <summary>Verifies the arg can clone.</summary>
        [TestMethod]
        public void Arg_Cloneable()
        {
            string value = Tools.Randomizer.Create<string>();
            Arg origin = Arg.LambdaWhere<string>(d => d == value);
            Arg copy = Tools.Duplicator.Copy(origin);

            Tools.Asserter.Is(origin, copy);
            Tools.Asserter.Is(true, copy.Matches(value));
            Tools.Asserter.Is(false, copy.Matches(Tools.Randiffer.Branch(value)));
        }
    }
}
