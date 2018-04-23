using System;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool
{
    /// <summary>Verifies behavior.</summary>
    public static class DuplicatorChainerTests
    {
        /// <summary>Verifies callback must be provided.</summary>
        [Fact]
        public static void New_NullsInvalid()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new DuplicatorChainer(null, (o, c) => null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new DuplicatorChainer(Tools.Duplicator, null));
        }
    }
}
