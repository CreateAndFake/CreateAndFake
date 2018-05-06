using System;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFakeTests.Toolbox.TesterTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    public static class TesterTests
    {
        /// <summary>Instance to test with.</summary>
        private static Tester s_ShortTestInstance = new Tester(Tools.Gen, Tools.Randomizer,
            Tools.Duplicator, Tools.Asserter, new TimeSpan(0, 0, 0, 0, 100));

        /// <summary>Instance to test with.</summary>
        private static Tester s_LongTestInstance = new Tester(Tools.Gen, Tools.Randomizer,
            Tools.Duplicator, Tools.Asserter, new TimeSpan(0, 0, 10));

        /// <summary>Verifies openness for custom individual behavior by inheritance.</summary>
        [Fact]
        public static void Tester_AllMethodsVirtual()
        {
            Tools.Asserter.IsEmpty(typeof(Tester)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsVirtual)
                .Select(m => m.Name));
        }

        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void Tester_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException(s_ShortTestInstance);
        }

        /// <summary>Verifies parameters are not mutated.</summary>
        [Fact]
        public static void Tester_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation(s_ShortTestInstance);
        }

        /// <summary>Verifies disposables are properly disposed.</summary>
        [Fact]
        public static void PreventsNullRefException_Disposes()
        {
            lock (MockDisposableSample.Lock)
            {
                MockDisposableSample.ClassDisposes = 0;
                MockDisposableSample.FinalizerDisposes = 0;
                MockDisposableSample.Fake = Tools.Faker.Stub<IDisposable>();

                s_LongTestInstance.PreventsNullRefException<MockDisposableSample>();
                Tools.Asserter.Is(3, MockDisposableSample.ClassDisposes);
                Tools.Asserter.Is(0, MockDisposableSample.FinalizerDisposes);
                MockDisposableSample.Fake.Verify(Times.Exactly(2), d => d.Dispose());
            }
        }

        /// <summary>Verifies disposables are properly disposed.</summary>
        [Fact]
        public static void PreventsParameterMutation_Disposes()
        {
            lock (MockDisposableSample.Lock)
            {
                MockDisposableSample.ClassDisposes = 0;
                MockDisposableSample.FinalizerDisposes = 0;
                MockDisposableSample.Fake = Tools.Faker.Stub<IDisposable>();

                s_LongTestInstance.PreventsParameterMutation<MockDisposableSample>();
                Tools.Asserter.Is(4, MockDisposableSample.ClassDisposes);
                Tools.Asserter.Is(0, MockDisposableSample.FinalizerDisposes);
                MockDisposableSample.Fake.Verify(Times.Exactly(2), d => d.Dispose());
            }
        }
    }
}
