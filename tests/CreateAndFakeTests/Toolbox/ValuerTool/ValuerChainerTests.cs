using System;
using CreateAndFake;
using CreateAndFake.Toolbox.ValuerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.ValuerTool
{
    /// <summary>Verifies behavior.</summary>
    public static class ValuerChainerTests
    {
        /// <summary>Verifies callback must be provided.</summary>
        [Fact]
        public static void New_NullsInvalid()
        {
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new ValuerChainer(null, (o, c) => 0, (e, a, c) => null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new ValuerChainer(Tools.Valuer, (o, c) => 0, null));
            Tools.Asserter.Throws<ArgumentNullException>(
                () => new ValuerChainer(Tools.Valuer, null, (e, a, c) => null));
        }
    }
}
