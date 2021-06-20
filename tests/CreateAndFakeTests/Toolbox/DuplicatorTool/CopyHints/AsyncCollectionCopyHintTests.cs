﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.DuplicatorTool.CopyHints;
using CreateAndFakeTests.TestBases;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Verifies behavior.</summary>
    public sealed class AsyncCollectionCopyHintTests : CopyHintTestBase<AsyncCollectionCopyHint>
    {
        /// <summary>Types that can be created by the hint.</summary>
        private static readonly Type[] _ValidTypes
            = new[] { typeof(IAsyncEnumerable<int>), typeof(IAsyncEnumerable<string>), typeof(IAsyncEnumerable<object>) };

        /// <summary>Types that can't be created by the hint.</summary>
        private static readonly Type[] _InvalidTypes = new[] { typeof(object), typeof(IEnumerable) };

        /// <summary>Sets up the tests.</summary>
        public AsyncCollectionCopyHintTests() : base(_ValidTypes, _InvalidTypes) { }

        [Theory, RandomData]
        internal static void TryCopy_Empty([Size(0)] IAsyncEnumerable<int> items)
        {
            Tools.Duplicator.Copy(items).Assert().Is(items);
        }

        [Fact]
        internal static async Task CopyAsync_Interrupt()
        {
            IAsyncEnumerable<int> original = Tools.Randomizer.CreateSized<IAsyncEnumerable<int>>(5);
            IAsyncEnumerable<int> items = Tools.Duplicator.Copy(original);

            await items.GetAsyncEnumerator().DisposeAsync().ConfigureAwait(false);

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
}
