using System.Collections;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <inheritdoc/>
    public sealed class AssertGroup : AssertGroupBase<AssertGroup>
    {
        /// <summary>Initializes a new instance of the <see cref="AssertGroup"/> class.</summary>
        /// <inheritdoc/>
        internal AssertGroup(IRandom gen, IValuer valuer, IEnumerable collection) : base(gen, valuer, collection) { }
    }
}
