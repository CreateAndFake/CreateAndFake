using System;
using System.Collections.Generic;
using System.Linq;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.RandifferTool
{
    /// <summary>Creates random variants of objects.</summary>
    public sealed class Randiffer : IRandiffer
    {
        /// <summary>Handles randomization.</summary>
        private readonly IRandomizer m_Randomizer;

        /// <summary>Ensures object variance.</summary>
        private readonly IValuer m_Valuer;

        /// <summary>Limits attempts at creating variants.</summary>
        private readonly Limiter m_Limiter;

        /// <summary>Sets up the randiffer capabilities.</summary>
        /// <param name="randomizer">Handles randomization.</param>
        /// <param name="valuer">Ensures object variance.</param>
        /// <param name="limiter">Limits attempts at creating variants.</param>
        public Randiffer(IRandomizer randomizer, IValuer valuer, Limiter limiter)
        {
            m_Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            m_Valuer = valuer ?? throw new ArgumentNullException(nameof(valuer));
            m_Limiter = limiter;
        }

        /// <summary>Creates a variant.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="instance">Object to diverge from.</param>
        /// <param name="extraInstances">Extra objects to diverge from.</param>
        /// <returns>The created instance.</returns>
        public T Branch<T>(T instance, params T[] extraInstances)
        {
            return (T)Branch(typeof(T), instance, extraInstances?.Cast<object>().ToArray());
        }

        /// <summary>Creates a variant.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="instance">Object to diverge from.</param>
        /// <param name="extraInstances">Extra objects to diverge from.</param>
        /// <returns>The created instance.</returns>
        public object Branch(Type type, object instance, params object[] extraInstances)
        {
            IEnumerable<object> values = (extraInstances ?? Enumerable.Empty<object>()).Prepend(instance);

            object result = default;
            try
            {
                m_Limiter.StallUntil(
                    () => result = m_Randomizer.Create(type),
                    () => values.All(o => !m_Valuer.Equals(result, o))).Wait();
            }
            catch (AggregateException e)
            {
                throw new TimeoutException($"Could not create different instance of type '{type}'.", e);
            }
            return result;
        }
    }
}
