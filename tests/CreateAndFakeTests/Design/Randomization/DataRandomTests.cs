using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.Design.Randomization
{
    /// <summary>Verifies behavior.</summary>
    public static class DataRandomTests
    {
        [Fact]
        internal static void DataRandom_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<DataRandom>();
        }

        [Theory, RandomData]
        internal static void DataRandom_MaintainsValues(DataRandom testInstance)
        {
            foreach (PropertyInfo prop in typeof(DataRandom).GetProperties())
            {
                prop.GetValue(testInstance).Assert().Is(prop.GetValue(testInstance));
            }
        }

        [Theory, RandomData]
        internal static void DataRandom_DataVaries(DataRandom testInstance)
        {
            Tools.Mutator.Variant(testInstance).Assert().IsNot(testInstance);
        }

        [Theory, RandomData]
        internal static void Find_AllSearchable(DataRandom testInstance)
        {
            foreach (string item in DataRandom.SupportedProperties)
            {
                testInstance.Find(item).Assert().IsNot(null);
            }
        }

        [Theory, RandomData]
        internal static void Find_IgnoresSpecialChars(DataRandom testInstance)
        {
            testInstance.Find("_" + DataRandom.SupportedProperties.First()).Assert().IsNot(null);
        }

        [Theory, RandomData]
        internal static void Find_NoResultNull(DataRandom testInstance, string name)
        {
            testInstance.Find(name).Assert().Is(null);
        }
    }
}
