using System;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFakeTests.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class FakeTests
    {
        /// <summary>Verifies a total does not been to be specified.</summary>
        [Fact]
        public static void Verify_NoTotalValid()
        {
            Tools.Faker.Mock<object>().VerifyAll();
        }

        /// <summary>Verifies protected methods are supported.</summary>
        [Fact]
        public static void SetupVerify_WorksProtectedMethods()
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
