﻿using System;
using CreateAndFake;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.TesterTool;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.TesterTool.TestSamples;
using Xunit;

namespace CreateAndFakeTests.Toolbox.TesterTool
{
    /// <summary>Verifies behavior.</summary>
    public static class ExceptionGuarderTests
    {
        [Fact]
        internal static void ExceptionGuarder_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<ExceptionGuarder>();
        }

        [Fact]
        internal static void ExceptionGuarder_NoParameterMutation()
        {
            Tools.Tester.PreventsParameterMutation<ExceptionGuarder>();
        }

        [Fact]
        internal static void CallAllMethods_SuccessfulNoException()
        {
            Tools.Tester.PassthroughWithNoExceptions<InjectMockSample>();
        }

        [Fact]
        internal static void CallAllMethods_FailsWithException()
        {
            Tools.Asserter.Throws<AssertException>(
                () => Tools.Tester.PassthroughWithNoExceptions<MethodThrowsSample>());
        }

        [Theory, RandomData]
        internal static void HandleCheckException_UsesAsserterFail(GenericFixer fixer, Fake<Asserter> asserter, TimeSpan timeout)
        {
            asserter.Setup(
                d => d.Fail(Arg.Any<Exception>(), Arg.Any<string>()),
                Behavior.None(Times.Once));

            new ExceptionGuarder(fixer, Tools.Randomizer, asserter.Dummy, timeout)
                .CallAllMethods(new MethodThrowsSample(), null);

            asserter.VerifyAll();
        }
    }
}
