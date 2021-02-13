using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using Xunit;

namespace CreateAndFakeTests.Toolbox.FakerTool.Proxy
{
    /// <summary>Verifies behavior.</summary>
    public static class FakeMetaProviderTests
    {
        [Fact]
        internal static void FakeMetaProvider_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<FakeMetaProvider>();
        }

        [Fact]
        internal static void FakeMetaProvider_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<FakeMetaProvider>();
        }

        [Theory, RandomData]
        internal static void Verify_PresetOutOfRangeThrows(string name)
        {
            FakeMetaProvider provider = new()
            {
                ThrowByDefault = false
            };

            CallData data = new(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);

            provider.SetCallBehavior(data, Behavior.None(Times.Once));
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());

            provider.CallVoid(null, Tools.Mutator.Variant(name), Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());

            provider.CallVoid(null, name, Type.EmptyTypes, Array.Empty<object>());
            provider.Verify();

            provider.CallVoid(null, name, Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());
        }

        [Theory, RandomData]
        internal static void Verify_CustomOutOfRangeThrows(string name)
        {
            FakeMetaProvider provider = new()
            {
                ThrowByDefault = false
            };

            CallData data = new(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);

            provider.Verify(0, data);
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(1, data));

            provider.CallVoid(null, Tools.Mutator.Variant(name), Type.EmptyTypes, Array.Empty<object>());
            provider.Verify(0, data);
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(1, data));

            provider.CallVoid(null, name, Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(0, data));
            provider.Verify(1, data);

            provider.CallVoid(null, name, Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(1, data));
            provider.Verify(2, data);
        }

        [Fact]
        internal static void VerifyTotalCalls_OutOfRangeThrows()
        {
            FakeMetaProvider provider = new()
            {
                ThrowByDefault = false
            };

            provider.VerifyTotalCalls(0);
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.VerifyTotalCalls(1));

            provider.CallVoid(null, Tools.Randomizer.Create<string>(), Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.VerifyTotalCalls(0));
            provider.VerifyTotalCalls(1);

            provider.CallVoid(null, Tools.Randomizer.Create<string>(), Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.VerifyTotalCalls(1));
            provider.VerifyTotalCalls(2);
        }

        [Theory, RandomData]
        internal static void CallVoid_ReturnValueThrows(string name)
        {
            FakeMetaProvider provider = new();

            CallData data = new(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);
            provider.SetCallBehavior(data, Behavior.Returns(""));

            Tools.Asserter.Throws<InvalidOperationException>(
                () => provider.CallVoid(null, name, Type.EmptyTypes, Array.Empty<object>()));
        }
    }
}
