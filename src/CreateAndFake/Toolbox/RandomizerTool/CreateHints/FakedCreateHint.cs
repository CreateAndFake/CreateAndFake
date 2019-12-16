using System;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of fakes for the randomizer.</summary>
    public sealed class FakedCreateHint : CreateHint<IFaked>
    {
        /// <summary>Creates a random instance of the type.</summary>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>Created instance.</returns>
        protected override IFaked Create(RandomizerChainer randomizer)
        {
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            Fake stub = randomizer.Stub(typeof(object));
            stub.Dummy.FakeMeta.Identifier = randomizer.Create<int>();
            return stub.Dummy;
        }
    }
}
