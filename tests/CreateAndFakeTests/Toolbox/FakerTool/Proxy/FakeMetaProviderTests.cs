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
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void FakeMetaProvider_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<FakeMetaProvider>();
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void FakeMetaProvider_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<FakeMetaProvider>();
        }

        /// <summary>Verifies the provided value is matched by calls.</summary>
        [Theory, RandomData]
        public static void Verify_PresetOutOfRangeThrows(string name)
        {
            FakeMetaProvider provider = new FakeMetaProvider
            {
                ThrowByDefault = false
            };

            CallData data = new CallData(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);

            provider.SetCallBehavior(data, Behavior.None(Times.Once));
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());

            provider.CallVoid(Tools.Mutator.Variant(name), Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());

            provider.CallVoid(name, Type.EmptyTypes, Array.Empty<object>());
            provider.Verify();

            provider.CallVoid(name, Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());
        }

        /// <summary>Verifies the provided value is matched by calls.</summary>
        [Theory, RandomData]
        public static void Verify_CustomOutOfRangeThrows(string name)
        {
            FakeMetaProvider provider = new FakeMetaProvider
            {
                ThrowByDefault = false
            };

            CallData data = new CallData(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);

            provider.Verify(0, data);
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(1, data));

            provider.CallVoid(Tools.Mutator.Variant(name), Type.EmptyTypes, Array.Empty<object>());
            provider.Verify(0, data);
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(1, data));

            provider.CallVoid(name, Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(0, data));
            provider.Verify(1, data);

            provider.CallVoid(name, Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(1, data));
            provider.Verify(2, data);
        }

        /// <summary>Verifies the provided value is matched by calls.</summary>
        [Fact]
        public static void VerifyTotalCalls_OutOfRangeThrows()
        {
            FakeMetaProvider provider = new FakeMetaProvider
            {
                ThrowByDefault = false
            };

            provider.VerifyTotalCalls(0);
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.VerifyTotalCalls(1));

            provider.CallVoid(Tools.Randomizer.Create<string>(), Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.VerifyTotalCalls(0));
            provider.VerifyTotalCalls(1);

            provider.CallVoid(Tools.Randomizer.Create<string>(), Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.VerifyTotalCalls(1));
            provider.VerifyTotalCalls(2);
        }

        /// <summary>Verifies matched call has no return.</summary>
        [Theory, RandomData]
        public static void CallVoid_ReturnValueThrows(string name)
        {
            FakeMetaProvider provider = new FakeMetaProvider();

            CallData data = new CallData(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);
            provider.SetCallBehavior(data, Behavior.Returns(""));

            Tools.Asserter.Throws<InvalidOperationException>(
                () => provider.CallVoid(name, Type.EmptyTypes, Array.Empty<object>()));
        }
    }
}
