﻿using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

public sealed class DelegateCreateHintTests : CreateHintTestBase<DelegateCreateHint>
{
    private static readonly Type[] _ActionTypes =
    [
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
    ];

    private static readonly Type[] _FuncTypes =
    [
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
    ];

    private static readonly DelegateCreateHint _TestInstance = new();

    private static readonly Type[] _ValidTypes =
    [
        typeof(Action<string, object, int>),
        typeof(Func<int, string, object>),
        typeof(Delegate),
        typeof(Action)
    ];

    private static readonly Type[] _InvalidTypes = [typeof(object)];

    public DelegateCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Fact]
    internal static void Create_HandlesAllDelegates()
    {
        foreach (Type type in _ActionTypes.Concat(_FuncTypes))
        {
            _TestInstance.TryCreate(type, CreateChainer()).Item1.Assert().Is(true);
        }
    }

    [Fact]
    internal static void Create_HandlesOutRef()
    {
        (bool, object) result = _TestInstance.TryCreate(typeof(Action<IOutRef>), CreateChainer());
        result.Item1.Assert().Is(true);

        Action<IOutRef> action = (Action<IOutRef>)result.Item2;

        OutRef<int> sampleInt = new();
        action.Invoke(sampleInt);
        sampleInt.Var.Assert().IsNot(default(int));

        OutRef<string> sampleString = new();
        action.Invoke(sampleString);
        sampleString.Var.Assert().IsNot(default(string));
    }
}
