using System.Collections;
using CreateAndFake.Toolbox.AsserterTool.Fluent;

namespace CreateAndFake.Fluent
{
    /// <summary>Provides fluent assertions.</summary>
    public static class AssertExtensions
    {
        /// <summary>Handles common test scenarios.</summary>
        /// <param name="actual">Object to test.</param>
        /// <returns>Asserter to test <paramref name="actual"/> with.</returns>
        public static AssertObject Assert(this object actual)
        {
            return new AssertObject(Tools.Gen, Tools.Valuer, actual);
        }

        /// <summary>Handles common test scenarios.</summary>
        /// <param name="collection">Collection to test.</param>
        /// <returns>Asserter to test <paramref name="collection"/> with.</returns>
        public static AssertGroup Assert(this IEnumerable collection)
        {
            return new AssertGroup(Tools.Gen, Tools.Valuer, collection);
        }
    }
}
