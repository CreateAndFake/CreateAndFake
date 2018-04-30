using System;
using System.Linq;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class DelegateCreateHintTests : CreateHintTestBase<DelegateCreateHint>
    {
        /// <summary>Possible action types to use.</summary>
        private static readonly Type[] s_ActionTypes = new[]
        {
            typeof(Action),
            typeof(Action<>),
            typeof(Action<,>),
            typeof(Action<,,>),
            typeof(Action<,,,>),
            typeof(Action<,,,,>),
            typeof(Action<,,,,,>),
            typeof(Action<,,,,,,>),
            typeof(Action<,,,,,,,>),
            typeof(Action<,,,,,,,,>),
            typeof(Action<,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,,>),
            typeof(Action<,,,,,,,,,,,,,,,>)
        };

        /// <summary>Possible func types to use.</summary>
        private static readonly Type[] s_FuncTypes = new[]
        {
            typeof(Func<>),
            typeof(Func<,>),
            typeof(Func<,,>),
            typeof(Func<,,,>),
            typeof(Func<,,,,>),
            typeof(Func<,,,,,>),
            typeof(Func<,,,,,,>),
            typeof(Func<,,,,,,,>),
            typeof(Func<,,,,,,,,>),
            typeof(Func<,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,,>),
            typeof(Func<,,,,,,,,,,,,,,,,>)
        };

        /// <summary>Instance to test with.</summary>
        private static readonly DelegateCreateHint s_TestInstance = new DelegateCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(Delegate),
            typeof(Action), typeof(Action<string, object, int>), typeof(Func<int, string, object>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public DelegateCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }

        /// <summary>Verifies every func and action type are handled.</summary>
        [Fact]
        public static void Create_HandlesAllDelegates()
        {
            foreach (Type type in s_ActionTypes.Concat(s_FuncTypes))
            {
                Tools.Asserter.Is(true, s_TestInstance.TryCreate(type, CreateChainer()).Item1);
            }
        }

        /// <summary>Verifies out is supported through OutRef.</summary>
        [Fact]
        public static void Create_HandlesOut()
        {
            (bool, object) result = s_TestInstance.TryCreate(typeof(Action<IOutRef>), CreateChainer());
            Tools.Asserter.Is(true, result.Item1);

            Action<IOutRef> action = (Action<IOutRef>)result.Item2;

            OutRef<int> sampleInt = new OutRef<int>();
            action.Invoke(sampleInt);
            Tools.Asserter.IsNot(default(int), sampleInt.Var);

            OutRef<string> sampleString = new OutRef<string>();
            action.Invoke(sampleString);
            Tools.Asserter.IsNot(default(string), sampleString.Var);
        }
    }
}
