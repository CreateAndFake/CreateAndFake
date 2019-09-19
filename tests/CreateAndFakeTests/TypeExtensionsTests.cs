using System;
using System.Collections.Generic;
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
        [Fact]
        internal static void TypeExtensions_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(typeof(TypeExtensions));
        }

        [Fact]
        internal static void TypeExtensions_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(typeof(TypeExtensions));
        }

        [Fact]
        internal static void Inherits_RaceConditionPrevented()
        {
            Type testType = Tools.Faker.Stub<object>(Assembly.GetExecutingAssembly()
                .GetTypes().Where(t => t.IsInterface).ToArray()).GetType();

            Parallel.For(0, 10, i => testType.Inherits<object>());
        }

        [Theory, RandomData]
        internal static void Inherits_RaceConditionPrevented2(IEnumerable<Type> types)
        {
            foreach (Type testType in types)
            {
                Parallel.For(0, 10, i => testType.Inherits<object>());
            }
        }
    }
}
