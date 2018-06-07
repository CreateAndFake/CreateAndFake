using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Toolbox;
using Xunit;
using TypeExtensions = CreateAndFake.Toolbox.TypeExtensions;

namespace CreateAndFakeTests
{
    /// <summary>Verifies behavior.</summary>
    public static class TypeExtensionsTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void TypeExtensions_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(typeof(TypeExtensions));
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void TypeExtensions_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(typeof(TypeExtensions));
        }

        /// <summary>Verifies no ill effects from cache lock race.</summary>
        [Fact]
        public static void Inherits_RaceConditionPrevented()
        {
            Type testType = Tools.Faker.Stub<object>(Assembly.GetExecutingAssembly()
                .GetTypes().Where(t => t.IsInterface).ToArray()).GetType();

            Parallel.For(0, 10, i => testType.Inherits<object>());
        }
    }
}
