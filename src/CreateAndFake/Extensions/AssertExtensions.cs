using System.Collections;
using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFake.Fluent
{
    /// <summary>Provides fluent assertions.</summary>
    public static class AssertExtensions
    {
        /// <summary>Handles common test scenarios.</summary>
        /// <param name="actual">Object to test.</param>
        /// <returns>Asserter to test with.</returns>
        public static AssertObject Assert(this object actual)
        {
            return new AssertObject(Tools.Valuer, actual);
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <param name="collection">Collection to test.</param>
        /// <returns>Asserter to test with.</returns>
        public static AssertCollection Assert(this IEnumerable collection)
        {
            return new AssertCollection(Tools.Valuer, collection);
        }
    }
}
