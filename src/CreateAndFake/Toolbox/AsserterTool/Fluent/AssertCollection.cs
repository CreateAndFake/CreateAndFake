using System.Collections;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls.</summary>
    public sealed class AssertCollection : AssertCollectionBase<AssertCollection>
    {
        /// <summary>Initializer.</summary>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="collection">Collection to check.</param>
        internal AssertCollection(IValuer valuer, IEnumerable collection) : base(valuer, collection) { }
    }
}
