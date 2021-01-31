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
        private static readonly Type[] _ActionTypes = new[]
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
        private static readonly Type[] _FuncTypes = new[]
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
        private static readonly DelegateCreateHint _TestInstance = new();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[] { typeof(Delegate),
            typeof(Action), typeof(Action<string, object, int>), typeof(Func<int, string, object>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public DelegateCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

        [Fact]
        internal static void Create_HandlesAllDelegates()
        {
            foreach (Type type in _ActionTypes.Concat(_FuncTypes))
            {
                Tools.Asserter.Is(true, _TestInstance.TryCreate(type, CreateChainer()).Item1);
            }
        }

        [Fact]
        internal static void Create_HandlesOutRef()
        {
            (bool, object) result = _TestInstance.TryCreate(typeof(Action<IOutRef>), CreateChainer());
            Tools.Asserter.Is(true, result.Item1);

            Action<IOutRef> action = (Action<IOutRef>)result.Item2;

            OutRef<int> sampleInt = new();
            action.Invoke(sampleInt);
            Tools.Asserter.IsNot(default(int), sampleInt.Var);

            OutRef<string> sampleString = new();
            action.Invoke(sampleString);
            Tools.Asserter.IsNot(default(string), sampleString.Var);
        }
    }
}
