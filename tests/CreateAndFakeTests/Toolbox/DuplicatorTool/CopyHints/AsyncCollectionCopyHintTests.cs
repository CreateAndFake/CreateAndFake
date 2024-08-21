using System.Collections;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Verifies behavior.</summary>
public sealed class AsyncCollectionCopyHintTests : CopyHintTestBase<AsyncCollectionCopyHint>
{
    /// <summary>Types that can be created by the hint.</summary>
    private static readonly Type[] _ValidTypes
        = [typeof(IAsyncEnumerable<int>), typeof(IAsyncEnumerable<string>), typeof(IAsyncEnumerable<object>)];

    /// <summary>Types that can't be created by the hint.</summary>
    private static readonly Type[] _InvalidTypes = [typeof(object), typeof(IEnumerable)];

    /// <summary>Sets up the tests.</summary>
    public AsyncCollectionCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }

    [Theory, RandomData]
    internal static void TryCopy_Empty([Size(0)] IAsyncEnumerable<int> items)
    {
        Tools.Duplicator.Copy(items).Assert().Is(items);
    }

    [Theory, RandomData]
    internal static async Task CopyAsync_Interrupt([Size(5)] IAsyncEnumerable<int> original)
    {
        IAsyncEnumerable<int> items = Tools.Duplicator.Copy(original);

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

        items.Assert().Is(original);
    }
}
