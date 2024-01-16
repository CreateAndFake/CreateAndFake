using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.RandomizerTool.CreateHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.RandomizerTool.CreateHints;

/// <summary>Verifies behavior.</summary>
public sealed class AsyncCollectionCreateHintTests : CreateHintTestBase<AsyncCollectionCreateHint>
{
    /// <summary>Instance to test with.</summary>
    private static readonly AsyncCollectionCreateHint _TestInstance = new();

    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes
        = [typeof(IAsyncEnumerable<int>), typeof(IAsyncEnumerable<string>), typeof(IAsyncEnumerable<object>)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes
        = [typeof(object), typeof(IEnumerable)];

    /// <summary>Sets up the tests.</summary>
    public AsyncCollectionCreateHintTests() : base(_TestInstance, _ValidTypes, _InvalidTypes) { }

    [Theory, RandomData]
    internal static async Task GetItems_Empty([Size(0)] IAsyncEnumerable<int> items)
    {
        IAsyncEnumerator<int> gen = items.GetAsyncEnumerator();
        await using (gen.ConfigureAwait(true))
        {
            (await gen.MoveNextAsync().ConfigureAwait(true)).Assert().Is(false);
        }
    }

    [Theory, RandomData]
    internal static async Task GetItems_Repeatable(IAsyncEnumerable<int> items)
    {
        List<int> first = [];
        await foreach (int item in items)
        {
            first.Add(item);
        }

        List<int> second = [];
        await foreach (int item in items)
        {
            second.Add(item);
        }

        first.Assert().Is(second);
    }

    [Theory, RandomData]
    internal static async Task GetItems_Cancel(IAsyncEnumerable<int> items)
    {
        await items.GetAsyncEnumerator().DisposeAsync().ConfigureAwait(true);
    }

    [Theory, RandomData]
    internal static async Task GetItems_Interrupt([Size(5)] IAsyncEnumerable<int> items)
    {
        await items.GetAsyncEnumerator().DisposeAsync().ConfigureAwait(true);

        int count = 0;
        await foreach (int item in items)
        {
            count++;
            if (count == 3)
            {
                break;
            }
        }

        count = 0;
        await foreach (int item in items)
        {
            count++;
        }
        count.Assert().Is(5);
    }
}
