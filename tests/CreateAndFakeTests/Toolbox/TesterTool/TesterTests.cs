using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.TesterTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class TesterTests
    {
        /// <summary>Verifies openness for custom individual behavior by inheritance.</summary>
        [TestMethod]
        public void Tester_AllMethodsVirtual()
        {
            MemberInfo[] nonVirtualMethods = typeof(Tester)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsVirtual && m.Name != "Is" && m.Name != "IsNot")
                .ToArray();

            if (nonVirtualMethods.Any())
            {
                Assert.Fail("Methods not virtual: " + string.Join(", ", nonVirtualMethods.Select(m => m.Name)));
            }
        }
    }
}
