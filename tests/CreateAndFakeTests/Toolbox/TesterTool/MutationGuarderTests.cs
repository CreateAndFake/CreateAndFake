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
        private static readonly MutationGuarder _ShortTestInstance = new MutationGuarder(
            new GenericFixer(Tools.Gen, Tools.Randomizer), Tools.Randomizer,
            Tools.Duplicator, Tools.Asserter, new TimeSpan(0, 0, 0, 0, 100));

        /// <summary>Instance to test with.</summary>
        private static readonly MutationGuarder _LongTestInstance = new MutationGuarder(
            new GenericFixer(Tools.Gen, Tools.Randomizer), Tools.Randomizer,
            Tools.Duplicator, Tools.Asserter, new TimeSpan(0, 0, 10));

        [Fact]
        internal static void MutationGuarder_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(_ShortTestInstance);
        }

        [Fact]
        internal static void MutationGuarder_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(_ShortTestInstance);
        }

        [Fact]
        internal static void CallMethod_TimesOut()
        {
            Tools.Asserter.Throws<TimeoutException>(() => _ShortTestInstance
                .PreventsMutationOnStatics(typeof(LongMethodSample), false));
        }

        [Fact]
        internal static void PreventsMutationOnConstructors_Disposes()
        {
            lock (MockDisposableSample.Lock)
            {
                MockDisposableSample.ClassDisposes = 0;
                MockDisposableSample.FinalizerDisposes = 0;
                MockDisposableSample.Fake = Tools.Faker.Stub<IDisposable>();

                _LongTestInstance.PreventsMutationOnConstructors(typeof(MockDisposableSample), true);
                Tools.Asserter.Is(2, MockDisposableSample.ClassDisposes);
                Tools.Asserter.Is(0, MockDisposableSample.FinalizerDisposes);
                MockDisposableSample.Fake.Verify(Times.Once, d => d.Dispose());
            }
        }

        [Fact]
        internal static void PreventsMutationOnMethods_Disposes()
        {
            lock (MockDisposableSample.Lock)
            {
                MockDisposableSample.ClassDisposes = 0;
                MockDisposableSample.FinalizerDisposes = 0;
                MockDisposableSample.Fake = Tools.Faker.Stub<IDisposable>();

                using (MockDisposableSample sample = new MockDisposableSample(null))
                {
                    _LongTestInstance.PreventsMutationOnMethods(sample);
                    Tools.Asserter.Is(1, MockDisposableSample.ClassDisposes);
                    Tools.Asserter.Is(0, MockDisposableSample.FinalizerDisposes);
                    MockDisposableSample.Fake.Verify(Times.Once, d => d.Dispose());
                }
            }
        }
    }
}
