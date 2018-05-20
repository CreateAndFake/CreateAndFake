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
        private static readonly CommonSystemCreateHint s_TestInstance = new CommonSystemCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[]
        {
            typeof(CultureInfo), typeof(TimeSpan), typeof(Assembly), typeof(AssemblyName), typeof(Type),
            typeof(Type).GetType(), typeof(ConstructorInfo), typeof(PropertyInfo), typeof(MethodInfo),
            typeof(MemberInfo), typeof(MemberInfo), typeof(FieldInfo), typeof(ParameterInfo), typeof(MethodBase)
        };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public CommonSystemCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }

        /// <summary>Verifies the hint keeps trying.</summary>
        [Fact]
        public static void TryCreate_ContinuesUntilMemberFound()
        {
            for (int i = 0; i < 50; i++)
            {
                s_TestInstance.TryCreate(typeof(FieldInfo), CreateChainer());
            }
        }
    }
}
