using System;

namespace CreateAndFake.Toolbox.RandifferTool
{
    /// <summary>Creates random variants of objects.</summary>
    public interface IRandiffer
    {
        /// <summary>Creates a variant.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="instance">Object to diverge from.</param>
        /// <param name="extraInstances">Extra objects to diverge from.</param>
        /// <returns>The created instance.</returns>
        T Branch<T>(T instance, params T[] extraInstances);

        /// <summary>Creates a variant.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="instance">Object to diverge from.</param>
        /// <param name="extraInstances">Extra objects to diverge from.</param>
        /// <returns>The created instance.</returns>
        object Branch(Type type, object instance, params object[] extraInstances);
    }
}
