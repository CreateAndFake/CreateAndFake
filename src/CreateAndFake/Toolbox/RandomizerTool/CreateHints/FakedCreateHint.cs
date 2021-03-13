using System;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of fakes for the randomizer.</summary>
    public sealed class FakedCreateHint : CreateHint<IFaked>
    {
        /// <inheritdoc/>
        protected override IFaked Create(RandomizerChainer randomizer)
        {
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            Fake stub = randomizer.Stub(typeof(object));
            stub.Dummy.FakeMeta.Identifier = randomizer.Create<int>();
            return stub.Dummy;
        }
    }
}
