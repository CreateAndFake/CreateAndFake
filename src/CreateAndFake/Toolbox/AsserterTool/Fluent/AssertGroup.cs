using System.Collections;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls.</summary>
    public sealed class AssertGroup : AssertGroupBase<AssertGroup>
    {
        /// <summary>Initializer.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="collection">Collection to check.</param>
        internal AssertGroup(IRandom gen, IValuer valuer, IEnumerable collection) : base(gen, valuer, collection) { }
    }
}
