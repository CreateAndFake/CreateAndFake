using System.Collections;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls.</summary>
    public sealed class AssertGroup : AssertCollectionBase<AssertGroup>
    {
        /// <summary>Initializer.</summary>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="collection">Collection to check.</param>
        internal AssertGroup(IValuer valuer, IEnumerable collection) : base(valuer, collection) { }
    }
}
