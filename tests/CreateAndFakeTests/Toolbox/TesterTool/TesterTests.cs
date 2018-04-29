using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.TesterTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    public static class TesterTests
    {
        /// <summary>Verifies openness for custom individual behavior by inheritance.</summary>
        [Fact]
        public static void Tester_AllMethodsVirtual()
        {
            Tools.Asserter.IsEmpty(typeof(Tester)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsVirtual)
                .Select(m => m.Name));
        }

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void Tester_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<Tester>();
        }
    }
}
