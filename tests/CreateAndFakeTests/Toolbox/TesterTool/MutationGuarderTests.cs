using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFakeTests.Toolbox.TesterTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    public static class MutationGuarderTests
    {
        /// <summary>Instance to test with.</summary>
        private static MutationGuarder s_TestInstance = new MutationGuarder(
            new GenericFixer(Tools.Gen, Tools.Randomizer), Tools.Randomizer,
            Tools.Duplicator, Tools.Asserter, new TimeSpan(0, 0, 0, 0, 100));

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void MutationGuarder_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(s_TestInstance);
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void MutationGuarder_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(s_TestInstance);
        }

        /// <summary>Verifies long methods time out.</summary>
        [Fact]
        public static void CallMethod_TimesOut()
        {
            Tools.Asserter.Throws<TimeoutException>(() => s_TestInstance
                .PreventsMutationOnStatics(typeof(LongMethodSample), false));
        }

        /// <summary>Verifies disposables are properly disposed.</summary>
        [Fact]
        public static void PreventsMutationOnConstructors_Disposes()
        {
            lock (MockDisposableSample.Lock)
            {
                MockDisposableSample.ClassDisposes = 0;
                MockDisposableSample.FinalizerDisposes = 0;
                MockDisposableSample.Fake = Tools.Faker.Stub<IDisposable>();

                s_TestInstance.PreventsMutationOnConstructors(typeof(MockDisposableSample), true);
                Tools.Asserter.Is(2, MockDisposableSample.ClassDisposes);
                Tools.Asserter.Is(0, MockDisposableSample.FinalizerDisposes);
                MockDisposableSample.Fake.Verify(Times.Once, d => d.Dispose());
            }
        }

        /// <summary>Verifies disposables are properly disposed.</summary>
        [Fact]
        public static void PreventsMutationOnMethods_Disposes()
        {
            lock (MockDisposableSample.Lock)
            {
                MockDisposableSample.ClassDisposes = 0;
                MockDisposableSample.FinalizerDisposes = 0;
                MockDisposableSample.Fake = Tools.Faker.Stub<IDisposable>();

                using (MockDisposableSample sample = new MockDisposableSample(null))
                {
                    s_TestInstance.PreventsMutationOnMethods(sample);
                    Tools.Asserter.Is(1, MockDisposableSample.ClassDisposes);
                    Tools.Asserter.Is(0, MockDisposableSample.FinalizerDisposes);
                    MockDisposableSample.Fake.Verify(Times.Once, d => d.Dispose());
                }
            }
        }
    }
}
