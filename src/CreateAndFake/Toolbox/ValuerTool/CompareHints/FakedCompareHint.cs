using System.Collections.Generic;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing fakes for the valuer.</summary>
    public sealed class FakedCompareHint : CompareHint<IFaked>
    {
        /// <summary>Finds the differences between two objects.</summary>
        /// <param name="expected">First object to compare.</param>
        /// <param name="actual">Second object to compare.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>Found differences.</returns>
        protected override IEnumerable<Difference> Compare(IFaked expected, IFaked actual, ValuerChainer valuer)
        {
            return valuer.Compare(expected.FakeMeta, actual.FakeMeta);
        }

        /// <summary>Calculates a hash code based upon value.</summary>
        /// <param name="item">Object to generate a code for.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The generated hash.</returns>
        protected override int GetHashCode(IFaked item, ValuerChainer valuer)
        {
            return valuer.GetHashCode(item.FakeMeta);
        }
    }
}
