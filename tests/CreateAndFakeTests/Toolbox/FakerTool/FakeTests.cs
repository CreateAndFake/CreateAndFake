﻿using System.Reflection;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.FakerTool;

public static class FakeTests
{
    [Fact]
    internal static void Verify_NoTotalValid()
    {
        Tools.Faker.Mock<object>().VerifyAll();
    }

    [Fact]
    internal static void SetupVerify_WorksProtectedMethods()
    {
        MethodInfo method = typeof(ProtectedSample)
            .GetMethod("ChildMethod", BindingFlags.Instance | BindingFlags.NonPublic);
        object[] args = [];

        Fake fake = Tools.Faker.Mock<ProtectedSample>();
        fake.Setup(method.Name, args, Behavior.None());
        fake.Verify(Times.Never, method.Name, args);

        method.Invoke(fake.Dummy, []);
        fake.Verify(Times.Once, method.Name, args);
    }
}
