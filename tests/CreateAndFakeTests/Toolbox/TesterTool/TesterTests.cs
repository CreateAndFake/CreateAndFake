using System;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFakeTests.Toolbox.TesterTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    public static class TesterTests
    {
        /// <summary>Instance to test with.</summary>
        private static readonly Tester _ShortTestInstance = new Tester(Tools.Gen, Tools.Randomizer,
            Tools.Duplicator, Tools.Asserter, new TimeSpan(0, 0, 0, 0, 100));

        /// <summary>Instance to test with.</summary>
        private static readonly Tester _LongTestInstance = new Tester(Tools.Gen, Tools.Randomizer,
            Tools.Duplicator, Tools.Asserter, new TimeSpan(0, 0, 10));

        [Fact]
        internal static void Tester_AllMethodsVirtual()
        {
            Tools.Asserter.IsEmpty(typeof(Tester)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsVirtual)
                .Select(m => m.Name));
        }

        [Theory, RandomData]
        internal static void Tester_GuardsNulls(IRandom gen, IRandomizer randomizer,
            IDuplicator duplicator, Asserter asserter, TimeSpan? timeout)
        {
            Tools.Asserter.Throws<ArgumentNullException>(() => new Tester(null, randomizer, duplicator, asserter, timeout));
            Tools.Asserter.Throws<ArgumentNullException>(() => new Tester(gen, null, duplicator, asserter, timeout));
            Tools.Asserter.Throws<ArgumentNullException>(() => new Tester(gen, randomizer, null, asserter, timeout));
            Tools.Asserter.Throws<ArgumentNullException>(() => new Tester(gen, randomizer, duplicator, null, timeout));

            Tools.Asserter.Throws<ArgumentNullException>(() => _ShortTestInstance.PreventsNullRefException(null));
            Tools.Asserter.Throws<ArgumentNullException>(() => _ShortTestInstance.PreventsParameterMutation(null));
        }

        [Fact]
        internal static void PreventsNullRefException_Disposes()
        {
            lock (MockDisposableSample._Lock)
            {
                MockDisposableSample._ClassDisposes = 0;
                MockDisposableSample._FinalizerDisposes = 0;
                MockDisposableSample._Fake = Tools.Faker.Stub<IDisposable>();

                _LongTestInstance.PreventsNullRefException<MockDisposableSample>();
                Tools.Asserter.Is(2, MockDisposableSample._ClassDisposes);
                Tools.Asserter.Is(0, MockDisposableSample._FinalizerDisposes);
                MockDisposableSample._Fake.Verify(Times.Exactly(2), d => d.Dispose());
            }
        }

        [Fact]
        internal static void PreventsParameterMutation_Disposes()
        {
            lock (MockDisposableSample._Lock)
            {
                MockDisposableSample._ClassDisposes = 0;
                MockDisposableSample._FinalizerDisposes = 0;
                MockDisposableSample._Fake = Tools.Faker.Stub<IDisposable>();

                _LongTestInstance.PreventsParameterMutation<MockDisposableSample>();
                Tools.Asserter.Is(2, MockDisposableSample._ClassDisposes);
                Tools.Asserter.Is(0, MockDisposableSample._FinalizerDisposes);
                MockDisposableSample._Fake.Verify(Times.Exactly(2), d => d.Dispose());
            }
        }
    }
}
