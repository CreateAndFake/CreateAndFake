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
        /// <summary>Verifies data is required.</summary>
        [Fact]
        public static void SetCallBehavior_NullsThrow()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new FakeMetaProvider().SetCallBehavior(null, Tools.Randomizer.Create<Behavior>()));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new FakeMetaProvider().SetCallBehavior(Tools.Randomizer.Create<CallData>(), null));
        }

        /// <summary>Verifies a null isn't accepted.</summary>
        [Fact]
        public static void DeepClone_NullThrows()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => Tools.Randomizer.Create<FakeMetaProvider>().DeepClone(null));
        }

        /// <summary>Verifies a call data is required.</summary>
        [Fact]
        public static void Verify_NullCallDataThrows()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new FakeMetaProvider().Verify(0, null));
        }

        /// <summary>Verifies the provided value is matched by calls.</summary>
        [Fact]
        public static void Verify_PresetOutOfRangeThrows()
        {
            FakeMetaProvider provider = new FakeMetaProvider
            {
                ThrowByDefault = false
            };

            string name = Tools.Randomizer.Create<string>();
            CallData data = new CallData(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);

            provider.SetCallBehavior(data, Behavior.None(Times.Once));
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());

            provider.CallVoid(Tools.Randiffer.Branch(name), Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());

            provider.CallVoid(name, Type.EmptyTypes, Array.Empty<object>());
            provider.Verify();

            provider.CallVoid(name, Type.EmptyTypes, Array.Empty<object>());
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify());
        }

        /// <summary>Verifies the provided value is matched by calls.</summary>
        [Fact]
        public static void Verify_CustomOutOfRangeThrows()
        {
            FakeMetaProvider provider = new FakeMetaProvider
            {
                ThrowByDefault = false
            };

            string name = Tools.Randomizer.Create<string>();
            CallData data = new CallData(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);

            provider.Verify(0, data);
            Tools.Asserter.Throws<FakeVerifyException>(() => provider.Verify(1, data));

            provider.CallVoid(Tools.Randiffer.Branch(name), Type.EmptyTypes, Array.Empty<object>());
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
        [Fact]
        public static void CallVoid_ReturnValueThrows()
        {
            FakeMetaProvider provider = new FakeMetaProvider();
            string name = Tools.Randomizer.Create<string>();

            CallData data = new CallData(name, Type.EmptyTypes, Array.Empty<object>(), Tools.Valuer);
            provider.SetCallBehavior(data, Behavior.Returns(""));

            Tools.Asserter.Throws<InvalidOperationException>(
                () => provider.CallVoid(name, Type.EmptyTypes, Array.Empty<object>()));
        }
    }
}
