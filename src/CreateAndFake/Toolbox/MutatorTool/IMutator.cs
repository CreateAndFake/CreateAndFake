using System;

namespace CreateAndFake.Toolbox.MutatorTool
{
    /// <summary>Changes the value of objects or creates alternatives.</summary>
    public interface IMutator
    {
        /// <summary>Creates an object with different values.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="instance">Object to diverge from.</param>
        /// <param name="extraInstances">Extra objects to diverge from.</param>
        /// <returns>The created instance.</returns>
        T Variant<T>(T instance, params T[] extraInstances);

        /// <summary>Creates an object with different values.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="instance">Object to diverge from.</param>
        /// <param name="extraInstances">Extra objects to diverge from.</param>
        /// <returns>The created instance.</returns>
        object Variant(Type type, object instance, params object[] extraInstances);
    }
}
