using System.Collections;
using CreateAndFake;
using CreateAndFake.Fluent;
using Xunit;

namespace CreateAndFakeTests.Extensions
{
    /// <summary>Verifies behavior.</summary>
    public static class AssertExtensionsTests
    {
        [Fact]
        internal static void AssertExtensions_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(typeof(AssertExtensions));
        }

        [Fact]
        internal static void AssertExtensions_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(typeof(AssertExtensions));
        }

        [Theory, RandomData]
        internal static void Assert_ObjectIsFluent(int data)
        {
            data.Assert().Is(data).And.IsNot(null);
        }

        [Theory, RandomData]
        internal static void Assert_StringIsFluent(string data)
        {
            data.Assert().Is(data).And.IsNotEmpty().And.HasCount(data.Length);
        }

        [Theory, RandomData]
        internal static void Assert_CollectionIsFluent(IEnumerable data)
        {
            data.Assert().IsNotEmpty().And.IsNot(null);
        }
    }
}
