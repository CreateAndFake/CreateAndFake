using System;
using System.Reflection;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class CommonSystemCreateHintTests : CreateHintTestBase<CommonSystemCreateHint>
    {
        /// <summary>Instance to test with.</summary>
        private static readonly CommonSystemCreateHint s_TestInstance = new CommonSystemCreateHint();

        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] s_ValidTypes = new[] { typeof(PropertyInfo),
            typeof(MemberInfo), typeof(FieldInfo), typeof(Type), typeof(TimeSpan) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] s_InvalidTypes = new[] { typeof(object) };

        /// <summary>Sets up the tests.</summary>
        public CommonSystemCreateHintTests() : base(s_TestInstance, s_ValidTypes, s_InvalidTypes) { }

        /// <summary>Verifies the hint keeps trying.</summary>
        [TestMethod]
        public void TryCreate_ContinuesUntilMemberFound()
        {
            for (int i = 0; i < 50; i++)
            {
                s_TestInstance.TryCreate(typeof(FieldInfo), CreateChainer());
            }
        }
    }
}
