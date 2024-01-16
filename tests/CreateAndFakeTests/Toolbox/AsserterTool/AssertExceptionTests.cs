using System;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.AsserterTool;

/// <summary>Verifies behavior.</summary>
public sealed class AssertExceptionTests : ExceptionTestBase<AssertException>
{
    [Fact]
    internal static void AssertException_UnknownMessageDefault()
    {
        Tools.Asserter.Is("Unknown assert failure.", new AssertException(null, null, null).Message);
    }

    [Theory, RandomData]
    internal static void AssertException_MessageFormat(
        string message, string details, string content, int seed, Exception ex)
    {
        new AssertException(message, null, null).Message.Contains(message).Assert().Is(true);
        new AssertException(message, null, seed).Message.Contains($"{seed}").Assert().Is(true);

        new AssertException(message, details, seed).Message.Contains(message).Assert().Is(true);
        new AssertException(message, details, seed).Message.Contains(details).Assert().Is(true);
        new AssertException(message, details, seed).Message.Contains($"{seed}").Assert().Is(true);
        new AssertException(message, details, seed, ex).Message.Contains(ex.ToString()).Assert().Is(false);

        new AssertException(message, null, seed, content).Message.Contains(message).Assert().Is(true);
        new AssertException(message, null, seed, content).Message.Contains(content).Assert().Is(true);
        new AssertException(message, null, seed, content).Message.Contains($"{seed}").Assert().Is(true);

        new AssertException(message, details, seed, content).Message.Contains(message).Assert().Is(true);
        new AssertException(message, details, seed, content).Message.Contains(details).Assert().Is(true);
        new AssertException(message, details, seed, content).Message.Contains(content).Assert().Is(true);
        new AssertException(message, details, seed, content).Message.Contains($"{seed}").Assert().Is(true);

        new AssertException(message, details, seed, content, ex).Message.Contains(message).Assert().Is(true);
        new AssertException(message, details, seed, content, ex).Message.Contains(details).Assert().Is(true);
        new AssertException(message, details, seed, content, ex).Message.Contains(content).Assert().Is(true);
        new AssertException(message, details, seed, content, ex).Message.Contains($"{seed}").Assert().Is(true);
        new AssertException(message, details, seed, content, ex).Message.Contains(ex.ToString()).Assert().Is(false);
    }
}
