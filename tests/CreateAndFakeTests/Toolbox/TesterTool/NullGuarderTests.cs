﻿using System;
using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFakeTests.Toolbox.TesterTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    public static class NullGuarderTests
    {
        /// <summary>Instance to test with.</summary>
        private static NullGuarder s_TestInstance = new NullGuarder(
            new GenericFixer(Tools.Gen, Tools.Randomizer),
            Tools.Randomizer, Tools.Asserter, new TimeSpan(0, 0, 0, 0, 100));

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void NullGuarder_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(s_TestInstance);
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void NullGuarder_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(s_TestInstance);
        }

        /// <summary>Verifies long methods time out.</summary>
        [Fact]
        public static void NullCheck_TimesOut()
        {
            Tools.Asserter.Throws<TimeoutException>(() => s_TestInstance
                .PreventsNullRefExceptionOnStatics(typeof(LongMethodSample), false));
        }

        /// <summary>Verifies null reference exceptions fail the test.</summary>
        [Fact]
        public static void NullCheck_NullReferenceThrows()
        {
            Tools.Asserter.Throws<AssertException>(() => s_TestInstance
                .PreventsNullRefExceptionOnConstructors(typeof(NullReferenceSample), true));
        }

        /// <summary>Verifies wrong parameter names on the guard fail the test.</summary>
        [Fact]
        public static void NullCheck_MismatchParamNameThrows()
        {
            Tools.Asserter.Throws<AssertException>(() => s_TestInstance
                .PreventsNullRefExceptionOnConstructors(typeof(MismatchParamNameSample), false));
        }

        /// <summary>Verifies disposables are properly disposed.</summary>
        [Fact]
        public static void PreventsNullRefExceptionOnConstructors_Disposes()
        {
            lock (MockDisposableSample.Lock)
            {
                MockDisposableSample.ClassDisposes = 0;
                MockDisposableSample.FinalizerDisposes = 0;
                MockDisposableSample.Fake = Tools.Faker.Stub<IDisposable>();

                s_TestInstance.PreventsNullRefExceptionOnConstructors(typeof(MockDisposableSample), true);
                Tools.Asserter.Is(2, MockDisposableSample.ClassDisposes);
                Tools.Asserter.Is(0, MockDisposableSample.FinalizerDisposes);
                MockDisposableSample.Fake.Verify(Times.Once, d => d.Dispose());
            }
        }

        /// <summary>Verifies disposables are properly disposed.</summary>
        [Fact]
        public static void PreventsNullRefExceptionOnMethods_Disposes()
        {
            lock (MockDisposableSample.Lock)
            {
                MockDisposableSample.ClassDisposes = 0;
                MockDisposableSample.FinalizerDisposes = 0;
                MockDisposableSample.Fake = Tools.Faker.Stub<IDisposable>();

                using (MockDisposableSample sample = new MockDisposableSample(null))
                {
                    s_TestInstance.PreventsNullRefExceptionOnMethods(sample);
                    Tools.Asserter.Is(0, MockDisposableSample.ClassDisposes);
                    Tools.Asserter.Is(0, MockDisposableSample.FinalizerDisposes);
                    MockDisposableSample.Fake.Verify(Times.Once, d => d.Dispose());
                }
            }
        }
    }
}
