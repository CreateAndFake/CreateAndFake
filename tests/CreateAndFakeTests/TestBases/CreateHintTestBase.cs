﻿using System;
using System.Collections;
using System.Collections.Generic;
using CreateAndFake;
using CreateAndFake.Design.Content;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.RandomizerTool;
using Xunit;

namespace CreateAndFakeTests.TestBases;

/// <summary>Handles testing create hints.</summary>
/// <typeparam name="T">Create hint to test.</typeparam>
public abstract class CreateHintTestBase<T> where T : CreateHint
{
    /// <summary>Instance to test with.</summary>
    protected T TestInstance { get; }

    /// <summary>Types that can be created by the hint.</summary>
    private readonly IEnumerable<Type> _validTypes;

    /// <summary>Types that can't be created by the hint.</summary>
    private readonly IEnumerable<Type> _invalidTypes;

    /// <summary>Sets up the tests.</summary>
    /// <param name="testInstance">Instance to test with.</param>
    /// <param name="validTypes">Types that can be created by the hint.</param>
    /// <param name="invalidTypes">Types that can't be created by the hint.</param>
    protected CreateHintTestBase(T testInstance, IEnumerable<Type> validTypes, IEnumerable<Type> invalidTypes)
    {
        TestInstance = testInstance;
        _validTypes = validTypes ?? Type.EmptyTypes;
        _invalidTypes = invalidTypes ?? Type.EmptyTypes;
    }

    /// <summary>Verifies null reference exceptions are prevented.</summary>
    [Fact]
    public void CreateHint_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException(TestInstance);
    }

    /// <summary>Verifies the hint supports the correct types.</summary>
    [Fact]
    public void TryCreate_SupportsValidTypes()
    {
        foreach (Type type in _validTypes)
        {
            (bool, object) result = TestInstance.TryCreate(type, CreateChainer());
            try
            {
                Tools.Asserter.Is(true, result.Item1,
                    "Hint '" + typeof(T).Name + "' did not support type '" + type.Name + "'.");
                Tools.Asserter.IsNot(null, result.Item2,
                    "Hint '" + typeof(T).Name + "' did not create valid '" + type.Name + "'.");

                if (result.Item2 is IEnumerable collection)
                {
                    Tools.Asserter.Is(true, collection.GetEnumerator().MoveNext(),
                        "Hint '" + typeof(T).Name + "' failed to create populated '" + type + "'.");
                }
            }
            finally
            {
                Disposer.Cleanup(result.Item2);
            }
        }
    }

    /// <summary>Verifies the hint doesn't support the wrong types.</summary>
    [Fact]
    public void TryCreate_InvalidTypesFail()
    {
        foreach (Type type in _invalidTypes)
        {
            Tools.Asserter.Is((false, (object)null), TestInstance.TryCreate(type, CreateChainer()),
                "Hint '" + typeof(T).Name + "' should not support type '" + type.Name + "'.");
        }
    }

    /// <returns>Chainer to use for testing.</returns>
    protected static RandomizerChainer CreateChainer()
    {
        return new RandomizerChainer(Tools.Faker, new FastRandom(), (t, c) => Tools.Randomizer.Create(t));
    }
}