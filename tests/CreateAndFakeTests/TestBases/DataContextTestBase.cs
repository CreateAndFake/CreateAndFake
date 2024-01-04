using System.Reflection;
using CreateAndFake;
using CreateAndFake.Design.Context;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.TestBases;

/// <summary>Handles testing data context classes.</summary>
/// <typeparam name="T">Type to test.</typeparam>
public abstract class DataContextTestBase<T> where T : BaseDataContext
{
    /// <summary>Verifies null reference exceptions are prevented.</summary>
    [Fact]
    public void DataContext_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException<PersonContext>();
    }

    /// <summary>Verifies data remains consistent on instance.</summary>
    [Theory, RandomData]
    public void DataContext_MaintainsValues(T testInstance)
    {
        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            prop.GetValue(testInstance).Assert().Is(prop.GetValue(testInstance));
        }
    }

    /// <summary>Verifies instances are not equal.</summary>
    [Theory, RandomData]
    public void DataContext_DataVaries(T testInstance)
    {
        Tools.Mutator.Variant(testInstance).Assert().IsNot(testInstance);
    }
}
