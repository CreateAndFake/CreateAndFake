using System;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls.</summary>
    public sealed class AssertObject : AssertObjectBase<AssertObject>
    {
        /// <summary>Sets up the asserter capabilities.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="actual">Object to compare with.</param>
        /// <exception cref="ArgumentNullException">If given a null valuer.</exception>
        internal AssertObject(IRandom gen, IValuer valuer, object actual) : base(gen, valuer, actual) { }
    }
}
