using System;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFakeTests.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class FakeTests
    {
        /// <summary>Verifies a total does not been to be specified.</summary>
        [TestMethod]
        public void Verify_NoTotalValid()
        {
            Tools.Faker.Mock<object>().Verify();
        }

        /// <summary>Verifies protected methods are supported.</summary>
        [TestMethod]
        public void SetupVerify_WorksProtectedMethods()
        {
            MethodInfo method = typeof(ProtectedSample)
                .GetMethod("ChildMethod", BindingFlags.Instance | BindingFlags.NonPublic);
            object[] args = Array.Empty<object>();

            Fake fake = Tools.Faker.Mock<ProtectedSample>();
            fake.Setup(method.Name, args, Behavior.None());
            fake.Verify(Times.Never, method.Name, args);

            method.Invoke(fake.Dummy, Array.Empty<object>());
            fake.Verify(Times.Once, method.Name, args);
        }
    }
}
