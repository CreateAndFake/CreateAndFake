using System;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.AsserterTool.Fluent;
using Xunit;

namespace CreateAndFakeTests.Extensions;

/// <summary>Verifies behavior.</summary>
public static class FakeExtensionsTests
{
    [Theory, RandomData]
    internal static void Called_PassesAsserters(int[] group, int comparable,
        string text, Action behavior, object item, [Fake] object data)
    {
        item.Assert().Called(data).And.Is(item);
        group.Assert().Called(data).And.Is(group);
        comparable.Assert().Called(data).And.Is(comparable);
        text.Assert().Called(data).And.Is(text);
        behavior.Assert().Called(data).And.Is(behavior);
    }

    [Theory, RandomData]
    internal static void Called_HandlesNullAsserters([Fake] object data)
    {
        ((AssertObject)null).Called(data);
        ((AssertGroup)null).Called(data);
        ((AssertComparable)null).Called(data);
        ((AssertText)null).Called(data);
        ((AssertBehavior)null).Called(data);
    }
}