using CreateAndFake.Design.Content;
using CreateAndFake.Toolbox.DuplicatorTool;

namespace CreateAndFakeTests.TestBases;

/// <summary>Handles testing copy hints.</summary>
/// <typeparam name="T">Copy hint to test.</typeparam>
public abstract class CopyHintTestBase<T> where T : CopyHint, new()
{
    /// <summary>Instance to test with.</summary>
    protected T TestInstance { get; } = new T();

    /// <summary>Types that can be copied by the hint.</summary>
    private readonly IEnumerable<Type> _validTypes;

    /// <summary>Types that can't be copied by the hint.</summary>
    private readonly IEnumerable<Type> _invalidTypes;

    /// <summary>If the hint copies by reference instead for value types.</summary>
    private readonly bool _copiesByRef;

    /// <summary>Sets up the tests.</summary>
    /// <param name="validTypes">Types that can be copied by the hint.</param>
    /// <param name="invalidTypes">Types that can't be copied by the hint.</param>
    /// <param name="copiesByRef">If the hint copies by reference instead for value types.</param>
    protected CopyHintTestBase(IEnumerable<Type> validTypes, IEnumerable<Type> invalidTypes, bool copiesByRef = false)
    {
        _validTypes = validTypes ?? Type.EmptyTypes;
        _invalidTypes = invalidTypes ?? Type.EmptyTypes;
        _copiesByRef = copiesByRef;
    }

    /// <summary>Verifies null reference exceptions are prevented.</summary>
    [Fact]
    public void CopyHint_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException(TestInstance);
    }

    /// <summary>Verifies the hint supports the correct types.</summary>
    [Fact]
    public void TryCopy_SupportsValidTypes()
    {
        foreach (Type type in _validTypes)
        {
            object data = null;
            (bool, object) result = (false, null);
            try
            {
                data = Tools.Randomizer.Create(type);
                result = TestInstance.TryCopy(data, CreateChainer());

                Tools.Asserter.Is((true, data), result,
                    "Hint '" + typeof(T).Name + "' failed to clone type '" + type.Name + "'.");

                if (_copiesByRef || data is string)
                {
                    Tools.Asserter.ReferenceEqual(data, result.Item2,
                        "Hint '" + typeof(T).Name + "' expected to copy value types by ref of type '" + type.Name + "'.");
                }
                else
                {
                    Tools.Asserter.ReferenceNotEqual(data, result.Item2,
                        "Hint '" + typeof(T).Name + "' copied by ref instead of a deep clone of type '" + type.Name + "'.");
                }
            }
            finally
            {
                Disposer.Cleanup(data, result.Item2);
            }
        }
    }

    /// <summary>Verifies the hint doesn't support the wrong types.</summary>
    [Fact]
    public void TryCopy_InvalidTypesFail()
    {
        foreach (Type type in _invalidTypes)
        {
            object data = Tools.Randomizer.Create(type);
            try
            {
                Tools.Asserter.Is((false, (object)null), TestInstance.TryCopy(data, CreateChainer()),
                    "Hint '" + typeof(T).Name + "' should not support type '" + type.Name + "'.");
            }
            finally
            {
                Disposer.Cleanup(data);
            }
        }
    }

    /// <returns>Chainer to use for testing.</returns>
    protected static DuplicatorChainer CreateChainer()
    {
        return new DuplicatorChainer(Tools.Duplicator, (o, c) => Tools.Duplicator.Copy(o));
    }
}
