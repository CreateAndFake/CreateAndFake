using System;
using System.Globalization;
using System.Reflection;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class CommonSystemCreateHintTests : CreateHintTestBase<CommonSystemCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly CommonSystemCreateHint _TestInstance = new();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes = new[]
        {
            typeof(CultureInfo),
            typeof(TimeSpan),
            typeof(DateTime),
            typeof(Assembly),
            typeof(AssemblyName),
            typeof(Guid),
            typeof(Type),
            typeof(Type).GetType(),
            typeof(ConstructorInfo),
            typeof(PropertyInfo),
            typeof(MethodInfo),
            typeof(MemberInfo),
            typeof(MemberInfo),
            typeof(FieldInfo),
            typeof(ParameterInfo),
            typeof(MethodBase),
            typeof(DateTimeOffset),
            typeof(Uri),
            typeof(UriBuilder)
        };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public CommonSystemCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

        [Fact]
        internal static void TryCreate_ContinuesUntilMemberFound()
        {
            for (int i = 0; i < 50; i++)
            {
                _TestInstance.TryCreate(typeof(FieldInfo), CreateChainer());
            }
        }
    }
}
