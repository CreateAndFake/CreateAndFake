using System;
using System.Collections;
using System.Reflection;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Creates objects and populates them with random values.</summary>
    public interface IRandomizer
    {
        /// <summary>Creates a randomized instance.</summary>
        /// <typeparam name="T">Type to create.</typeparam>
        /// <param name="condition">Optional condition for the instance to match.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        /// <exception cref="TimeoutException">If an instance couldn't be created to match the condition.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        T Create<T>(Func<T, bool> condition = null);

        /// <summary>Creates a randomized instance.</summary>
        /// <param name="condition">Optional condition for the instance to match.</param>
        /// <param name="type">Type to create.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        object Create(Type type, Func<object, bool> condition = null);

        /// <summary>Creates a randomized collection of the given size.</summary>
        /// <typeparam name="T">Collection type to create.</typeparam>
        /// <param name="count">Number of items to generate.</param>
        /// <returns>The created collection.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        T CreateSized<T>(int count) where T : IEnumerable;

        /// <summary>Creates a randomized collection of the given size.</summary>
        /// <param name="type">Collection type to create.</param>
        /// <param name="count">Number of items to generate.</param>
        /// <returns>The created collection.</returns>
        /// <exception cref="NotSupportedException">If no hint supports generating the type.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        object CreateSized(Type type, int count);

        /// <summary>
        ///     Constructs the parameters for a method.
        ///     Randomizes types by default.
        ///     Earlier types will be used to construct later types if possible.
        /// </summary>
        /// <param name="method">Method to create parameters for.</param>
        /// <param name="values">Starting values to inject into the instance.</param>
        /// <returns>Parameter arguments in order.</returns>
        MethodCallWrapper CreateFor(MethodBase method, params object[] values);

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

        /// <summary>Adds the hint to be used for randomization.</summary>
        /// <param name="hint">Hint to add.</param>
        /// <remarks>Should only be modified in module inintializers.</remarks>
        void AddHint(CreateHint hint);
    }
}
