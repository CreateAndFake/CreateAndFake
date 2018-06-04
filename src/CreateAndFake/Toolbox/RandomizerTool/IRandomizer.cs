using System;
using System.Reflection;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Creates objects and populates them with random values.</summary>
    public interface IRandomizer
    {
        /// <summary>Creates a randomized instance.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        T Create<T>();

        /// <summary>Creates a randomized instance.</summary>
        /// <param name="type">Type to create.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        object Create(Type type);

        /// <summary>
        ///     Constructs the parameters for a method.
        ///     Randomizes types by default.
        ///     Earlier types will be used to construct later types if possible.
        /// </summary>
        /// <param name="method">Method to create parameters for.</param>
        /// <returns>Parameter arguments in order.</returns>
        object[] CreateFor(MethodBase method);

        /// <summary>Creates an instance using the values or random data as needed.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="values">Values to inject into the instance.</param>
        /// <returns>The created instance.</returns>
        T Inject<T>(params object[] values);

        /// <summary>Creates an instance using the values or random data as needed.</summary>
        /// <param name="type">Type to create.</param>
        /// <param name="values">Values to inject into the instance.</param>
        /// <returns>The created instance.</returns>
        object Inject(Type type, params object[] values);
    }
}
