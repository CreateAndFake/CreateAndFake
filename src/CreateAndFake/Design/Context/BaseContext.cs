using System;
using CreateAndFake.Design.Randomization;

namespace CreateAndFake.Design.Context
{
    /// <summary>For generating values from data pools.</summary>
    public abstract class BaseDataContext
    {
        /// <inheritdoc cref="IRandom"/>
        protected IRandom Gen { get; }

        /// <summary>Sets up the context.</summary>
        /// <param name="gen">Handles basic randomization.</param>
        internal BaseDataContext(IRandom gen)
        {
            Gen = gen ?? throw new ArgumentNullException(nameof(gen));
        }
    }
}