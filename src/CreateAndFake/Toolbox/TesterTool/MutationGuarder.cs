using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates parameter mutation checks.</summary>
    internal sealed class MutationGuarder
    {
        /// <summary>Handles generic resolution.</summary>
        private readonly GenericFixer m_Fixer;

        /// <summary>Creates objects and populates them with random values.</summary>
        private readonly IRandomizer m_Randomizer;

        /// <summary>Deep clones objects.</summary>
        private readonly IDuplicator m_Duplicator;

        /// <summary>Handles common test scenarios.</summary>
        private readonly Asserter m_Asserter;

        /// <summary>How long to wait for methods to complete.</summary>
        private readonly TimeSpan m_Timeout;

        /// <summary>Sets up the guarder capabilities.</summary>
        /// <param name="fixer">Handles generic resolution.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="duplicator">Deep clones objects.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <param name="timeout">How long to wait for methods to complete.</param>
        internal MutationGuarder(GenericFixer fixer, IRandomizer randomizer,
            IDuplicator duplicator, Asserter asserter, TimeSpan timeout)
        {
            m_Fixer = fixer ?? throw new ArgumentNullException(nameof(fixer));
            m_Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            m_Duplicator = duplicator ?? throw new ArgumentNullException(nameof(duplicator));
            m_Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
            m_Timeout = timeout;
        }

        /// <summary>Verifies mutations are prevented on constructors.</summary>
        /// <param name="type">Type to verify.</param>
        /// <param name="callAllMethods">Run instance methods to validate constructor parameters.</param>
        internal void PreventsMutationOnConstructors(Type type, bool callAllMethods)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (ConstructorInfo constructor in type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => c.IsPublic || c.IsAssembly || c.IsFamily || c.IsFamilyOrAssembly)
                .Where(c => !c.IsPrivate))
            {
                PreventsMutation(null, constructor, callAllMethods);
            }
        }

        /// <summary>Verifies mutations are prevented on methods.</summary>
        /// <param name="instance">Instance to test the methods on.</param>
        internal void PreventsMutationOnMethods(object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            foreach (MethodInfo method in instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => !m.IsPrivate))
            {
                PreventsMutation(instance, m_Fixer.FixMethod(method), false);
            }
        }

        /// <summary>Verifies mutations are prevented on methods.</summary>
        /// <param name="type">Type to verify.</param>
        /// <param name="callAllMethods">Run instance methods to validate factory parameters.</param>
        internal void PreventsMutationOnStatics(Type type, bool callAllMethods)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (MethodInfo method in type
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => !m.IsPrivate))
            {
                PreventsMutation(null, m_Fixer.FixMethod(method),
                    callAllMethods && method.ReturnType.Inherits(type));
            }
        }

        /// <summary>Verifies mutations are prevented on a method.</summary>
        /// <param name="instance">Instance with the method under test.</param>
        /// <param name="method">Method under test.</param>
        /// <param name="callAllMethods">If all instance methods should be called after the method.</param>
        private void PreventsMutation(object instance, MethodBase method, bool callAllMethods)
        {
            object[] data = method.GetParameters()
                .Select(p => (!p.ParameterType.IsByRef) ? m_Randomizer.Create(p.ParameterType) : null)
                .ToArray();
            object[] copy = m_Duplicator.Copy(data);

            object result;
            if (instance == null && method is ConstructorInfo builder)
            {
                result = CallMethod(method, () => builder.Invoke(data));
            }
            else
            {
                result = CallMethod(method, () => method.Invoke(instance, data));
            }

            if (result != null && callAllMethods)
            {
                CallAllMethods(method, result);
            }
            (result as IDisposable)?.Dispose();

            m_Asserter.ValuesEqual(copy, data, $"Parameter data was mutated when testing '{method.Name}'.");
        }

        /// <summary>Calls all methods to test constructor parameter being set to null.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="instance">Instance whose methods to test.</param>
        private void CallAllMethods(MethodBase testOrigin, object instance)
        {
            foreach (MethodInfo method in instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => !m.IsPrivate)
                .Select(m => m_Fixer.FixMethod(m)))
            {
                (CallMethod(testOrigin, () => method.Invoke(instance, method.GetParameters()
                    .Select(p => m_Randomizer.Create(p.ParameterType)).ToArray())) as IDisposable)?.Dispose();
            }
        }

        /// <summary>Handles the call to a method.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="call">Call to invoke and test.</param>
        /// <returns>Returned result from the call.</returns>
        private object CallMethod(MethodBase testOrigin, Func<object> call)
        {
            try
            {
                Task<object> task = Task.Run(() =>
                {
                    object result = call.Invoke();
                    if (result is IEnumerable collection)
                    {
                        // Required to run through yield return methods.
                        return collection?.OfType<object>()?.ToArray();
                    }
                    else
                    {
                        return result;
                    }
                });
                if (!task.Wait(m_Timeout))
                {
                    throw new TimeoutException($"Attempting to run method '{testOrigin.Name}' timed out.");
                }
                return task.Result;
            }
            catch (AggregateException)
            {
                return null;
            }
        }
    }
}
