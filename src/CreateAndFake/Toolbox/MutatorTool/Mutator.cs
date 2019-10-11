using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.RandomizerTool;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.MutatorTool
{
    /// <summary>Changes the value of objects or creates alternatives.</summary>
    public sealed class Mutator : IMutator
    {
        /// <summary>Handles randomization.</summary>
        private readonly IRandomizer _randomizer;

        /// <summary>Ensures object variance.</summary>
        private readonly IValuer _valuer;

        /// <summary>Limits attempts at creating variants.</summary>
        private readonly Limiter _limiter;

        /// <summary>Sets up the randiffer capabilities.</summary>
        /// <param name="randomizer">Handles randomization.</param>
        /// <param name="valuer">Ensures object variance.</param>
        /// <param name="limiter">Limits attempts at creating variants.</param>
        public Mutator(IRandomizer randomizer, IValuer valuer, Limiter limiter)
        {
            _randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            _valuer = valuer ?? throw new ArgumentNullException(nameof(valuer));
            _limiter = limiter ?? throw new ArgumentNullException(nameof(limiter));
        }

        /// <summary>Creates an object with different values.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="instance">Object to diverge from.</param>
        /// <param name="extraInstances">Extra objects to diverge from.</param>
        /// <returns>The created instance.</returns>
        public T Variant<T>(T instance, params T[] extraInstances)
        {
            return (T)Variant(typeof(T), instance, extraInstances?.Cast<object>().ToArray());
        }

        /// <summary>Creates an object with different values.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="instance">Object to diverge from.</param>
        /// <param name="extraInstances">Extra objects to diverge from.</param>
        /// <returns>The created instance.</returns>
        public object Variant(Type type, object instance, params object[] extraInstances)
        {
            IEnumerable<object> values = (extraInstances ?? Enumerable.Empty<object>()).Prepend(instance);

            object result = default;
            try
            {
                _limiter.StallUntil(
                    () => result = _randomizer.Create(type),
                    () => values.All(o => !_valuer.Equals(result, o))).Wait();
            }
            catch (AggregateException e)
            {
                throw new TimeoutException($"Could not create different instance of type '{type}'.", e);
            }
            return result;
        }

        /// <summary>Attempts to mutate an object.</summary>
        /// <param name="instance">Object to modify.</param>
        /// <returns>True if modified; false otherwise.</returns>
        public bool Modify(object instance)
        {
            if (instance == null) return false;

            bool modified = false;

            Type type = instance.GetType();
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                field.SetValue(instance, Variant(field.FieldType, field.GetValue(instance)));
                modified = true;
            }
            foreach (PropertyInfo property in type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite && p.CanRead)
                .Where(p => p.GetGetMethod() != null)
                .Where(p => p.GetSetMethod() != null))
            {
                property.SetValue(instance, Variant(property.PropertyType, property.GetValue(instance)));
                modified = true;
            }

            return modified;
        }
    }
}
