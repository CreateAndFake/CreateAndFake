using System;
using System.Collections.Generic;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.ValuerTool.CompareHints
{
    /// <summary>Handles comparing fakes for <see cref="IValuer"/>.</summary>
    public sealed class FakedCompareHint : CompareHint<IFaked>
    {
        /// <inheritdoc/>
        protected override IEnumerable<Difference> Compare(IFaked expected, IFaked actual, ValuerChainer valuer)
        {
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return valuer.Compare(expected?.FakeMeta, actual?.FakeMeta);
        }

        /// <inheritdoc/>
        protected override int GetHashCode(IFaked item, ValuerChainer valuer)
        {
            if (valuer == null) throw new ArgumentNullException(nameof(valuer));

            return valuer.GetHashCode(item?.FakeMeta);
        }
    }
}
