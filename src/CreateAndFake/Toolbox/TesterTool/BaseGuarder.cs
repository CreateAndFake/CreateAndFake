using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates checks.</summary>
    internal abstract class BaseGuarder
    {
        /// <summary>Handles generic resolution.</summary>
        protected GenericFixer Fixer { get; }

        /// <summary>Creates objects and populates them with random values.</summary>
        protected IRandomizer Randomizer { get; }

        /// <summary>How long to wait for methods to complete.</summary>
        protected TimeSpan Timeout { get; }

        /// <summary>Sets up the guarder capabilities.</summary>
        /// <param name="fixer">Handles generic resolution.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="timeout">How long to wait for methods to complete.</param>
        protected BaseGuarder(GenericFixer fixer, IRandomizer randomizer, TimeSpan timeout)
        {
            Fixer = fixer ?? throw new ArgumentNullException(nameof(fixer));
            Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            Timeout = timeout;
        }

        /// <summary>Gets all testable constructors on a type.</summary>
        /// <param name="type">Type with the constructors to test.</param>
        /// <returns>Found contructors.</returns>
        protected static IEnumerable<ConstructorInfo> FindAllConstructors(Type type)
        {
            return type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => c.IsPublic || c.IsAssembly || c.IsFamilyOrAssembly)
                .Where(c => !c.IsPrivate);
        }

        /// <summary>Gets all testable methods on a type.</summary>
        /// <param name="type">Type with the methods to test.</param>
        /// <param name="kind">Instance, static, or both.</param>
        /// <returns>Found methods.</returns>
        protected static IEnumerable<MethodInfo> FindAllMethods(Type type, BindingFlags kind)
        {
            return type
                .GetMethods(kind | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => !m.IsPrivate);
        }

        /// <summary>Calls all methods to test parameter being set to null.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="instance">Instance whose methods to test.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        protected void CallAllMethods(MethodBase testOrigin,
            ParameterInfo testParam, object instance, object[] injectionValues)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            foreach (MethodInfo method in instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => m.Name != "Finalize" && m.Name != "Dispose")
                .Where(m => !m.IsPrivate)
                .Select(m => Fixer.FixMethod(m)))
            {
                (RunCheck(testOrigin, testParam,
                    () => method.Invoke(instance, method.GetParameters().Select(
                        p => Randomizer.Inject(p.ParameterType, injectionValues)).ToArray())) as IDisposable)?.Dispose();
            }
        }

        /// <summary>Runs the check.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="call">Call to invoke and test.</param>
        /// <returns>Returned result from the call.</returns>
        protected object RunCheck(MethodBase testOrigin, ParameterInfo testParam, Func<object> call)
        {
            if (testOrigin == null) throw new ArgumentNullException(nameof(testOrigin));
            if (call == null) throw new ArgumentNullException(nameof(call));
            try
            {
                Task<object> task = Task.Run(() =>
                {
                    object result = call.Invoke();
                    if (result is IEnumerable collection)
                    {
                        // Required to run through yield return methods.
                        return collection.OfType<object>().ToArray();
                    }
                    else
                    {
                        return result;
                    }
                });
                if (!task.Wait(Timeout))
                {
                    throw new TimeoutException($"Attempting to run method '{testOrigin.Name}' timed out.");
                }
                return task.Result;
            }
            catch (AggregateException taskException)
            {
                HandleCheckException(testOrigin, testParam, taskException);
                return null;
            }
        }

        /// <summary>Handles exceptions encountered by the check.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="taskException">Exception encountered.</param>
        protected abstract void HandleCheckException(MethodBase testOrigin,
            ParameterInfo testParam, AggregateException taskException);
    }
}
