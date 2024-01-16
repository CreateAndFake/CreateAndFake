using System;
using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.TesterTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool;

/// <summary>Verifies behavior.</summary>
public static class NullGuarderTests
{
    /// <summary>Instance to test with.</summary>
    private static readonly NullGuarder _ShortTestInstance = new(
        new GenericFixer(Tools.Gen, Tools.Randomizer),
        Tools.Randomizer, Tools.Asserter, new TimeSpan(0, 0, 0, 0, 100));

    /// <summary>Instance to test with.</summary>
    private static readonly NullGuarder _LongTestInstance = new(
        new GenericFixer(Tools.Gen, Tools.Randomizer),
        Tools.Randomizer, Tools.Asserter, new TimeSpan(0, 0, 10));

    [Fact]
    internal static void NullGuarder_GuardsNulls()
    {
        Tools.Tester.PreventsNullRefException(_ShortTestInstance);
    }

    [Fact]
    internal static void NullGuarder_NoParameterMutation()
    {
        Tools.Tester.PreventsParameterMutation(_ShortTestInstance);
    }

    [Fact]
    internal static void NullCheck_TimesOut()
    {
        Tools.Asserter.Throws<TimeoutException>(() => _ShortTestInstance
            .PreventsNullRefExceptionOnStatics(typeof(LongMethodSample), false));
    }

    [Fact]
    internal static void NullCheck_NullReferenceThrows()
    {
        Tools.Asserter.Throws<AssertException>(() => _ShortTestInstance
            .PreventsNullRefExceptionOnConstructors(typeof(NullReferenceSample), true));
    }

    [Theory, RandomData]
    internal static void PreventsNullRefException_InjectsMultipleValues(Fake<IOnlyMockSample> fake1, Fake<IOnlyMockSample> fake2)
    {
        Tools.Tester.PreventsNullRefException<InjectMockSample>(fake1, fake2);
    }

    [Theory, RandomData]
    internal static void PreventsNullRefException_InjectsWithMethods(Fake<IOnlyMockSample> fake)
    {
        Tools.Tester.PreventsNullRefException<MockMethodPassOnly>(fake);
    }

    [Fact]
    internal static void PreventsNullRefException_OnStatics()
    {
        Tools.Asserter.Throws<AssertException>(() =>
            Tools.Tester.PreventsNullRefException(typeof(StaticMutationSample)));
    }

    [Fact]
    internal static void PreventsNullRefException_StatelessFine()
    {
        Tools.Tester.PreventsNullRefException<StatelessSample>();
    }

    [Fact]
    internal static void PreventsNullRefException_NullInjectionsFine()
    {
        Tools.Tester.PreventsNullRefException<StatelessSample>((object[])null);
    }

    [Fact]
    internal static void PreventsNullRefExceptionOnConstructors_Disposes()
    {
        lock (MockDisposableSample._Lock)
        {
            MockDisposableSample._ClassDisposes = 0;
            MockDisposableSample._FinalizerDisposes = 0;
            MockDisposableSample._Fake = Tools.Faker.Stub<IDisposable>();

            _LongTestInstance.PreventsNullRefExceptionOnConstructors(typeof(MockDisposableSample), true);
            Tools.Asserter.Is(1, MockDisposableSample._ClassDisposes);
            Tools.Asserter.Is(0, MockDisposableSample._FinalizerDisposes);
            MockDisposableSample._Fake.Verify(Times.Once, d => d.Dispose());
        }
    }

    [Fact]
    internal static void PreventsNullRefExceptionOnMethods_Disposes()
    {
        lock (MockDisposableSample._Lock)
        {
            MockDisposableSample._ClassDisposes = 0;
            MockDisposableSample._FinalizerDisposes = 0;
            MockDisposableSample._Fake = Tools.Faker.Stub<IDisposable>();

            using (MockDisposableSample sample = new(null))
            {
                _LongTestInstance.PreventsNullRefExceptionOnMethods(sample);
                Tools.Asserter.Is(0, MockDisposableSample._ClassDisposes);
                Tools.Asserter.Is(0, MockDisposableSample._FinalizerDisposes);
                MockDisposableSample._Fake.Verify(Times.Once, d => d.Dispose());
            }
        }
    }
}
