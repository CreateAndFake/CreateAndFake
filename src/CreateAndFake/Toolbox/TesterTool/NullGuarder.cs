using System;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates null reference guard checks.</summary>
    internal sealed class NullGuarder : BaseGuarder
    {
        /// <summary>Handles common test scenarios.</summary>
        private readonly Asserter m_Asserter;

        /// <summary>Sets up the guarder capabilities.</summary>
        /// <param name="fixer">Handles generic resolution.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <param name="timeout">How long to wait for methods to complete.</param>
        internal NullGuarder(GenericFixer fixer, IRandomizer randomizer, Asserter asserter, TimeSpan timeout)
            : base(fixer, randomizer, timeout)
        {
            m_Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
        }

        /// <summary>
        ///     Verifies nulls are guarded on constructors.
        ///     Tests each nullable parameter possible with null.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </summary>
        /// <param name="type">Type to verify.</param>
        /// <param name="callAllMethods">Run instance methods to validate constructor parameters.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        internal void PreventsNullRefExceptionOnConstructors(
            Type type, bool callAllMethods, params object[] injectionValues)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (ConstructorInfo constructor in type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => c.IsPublic || c.IsAssembly || c.IsFamily || c.IsFamilyOrAssembly)
                .Where(c => !c.IsPrivate))
            {
                PreventsNullRefException(null, constructor, callAllMethods, injectionValues);
            }
        }

        /// <summary>
        ///     Verifies nulls are guarded on methods.
        ///     Tests each nullable parameter possible with null.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </summary>
        /// <param name="instance">Instance to test the methods on.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        internal void PreventsNullRefExceptionOnMethods(object instance, params object[] injectionValues)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            foreach (MethodInfo method in instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => m.Name != "Finalize" && m.Name != "Dispose")
                .Where(m => !m.IsPrivate))
            {
                PreventsNullRefException(instance, Fixer.FixMethod(method), false, injectionValues);
            }
        }

        /// <summary>
        ///     Verifies nulls are guarded on methods.
        ///     Tests each nullable parameter possible with null.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </summary>
        /// <param name="type">Type to verify.</param>
        /// <param name="callAllMethods">Run instance methods to validate factory parameters.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        internal void PreventsNullRefExceptionOnStatics(
            Type type, bool callAllMethods, params object[] injectionValues)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (MethodInfo method in type
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => !m.IsPrivate))
            {
                PreventsNullRefException(null, Fixer.FixMethod(method),
                    callAllMethods && method.ReturnType.Inherits(type), injectionValues);
            }
        }

        /// <summary>Verifies nulls are guarded on a method.</summary>
        /// <param name="instance">Instance with the method under test.</param>
        /// <param name="method">Method under test.</param>
        /// <param name="callAllMethods">If all instance methods should be called after the method.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        private void PreventsNullRefException(object instance,
            MethodBase method, bool callAllMethods, object[] injectionValues)
        {
            object[] data = method.GetParameters()
                .Select(p => (!p.ParameterType.IsByRef) ? Randomizer.Inject(p.ParameterType, injectionValues) : null)
                .ToArray();

            for (int i = 0; i < data.Length; i++)
            {
                ParameterInfo param = method.GetParameters()[i];
                if (param.ParameterType.IsValueType
                    && Nullable.GetUnderlyingType(param.ParameterType) == null)
                {
                    continue;
                }

                object original = data[i];
                data[i] = null;

                object result;
                if (instance == null && method is ConstructorInfo builder)
                {
                    result = RunCheck(method, param, () => builder.Invoke(data));
                }
                else
                {
                    result = RunCheck(method, param, () => method.Invoke(instance, data));
                }

                if (result != null && callAllMethods)
                {
                    CallAllMethods(method, param, result, injectionValues);
                }
                (result as IDisposable)?.Dispose();

                data[i] = original;
            }
        }

        /// <summary>Handles exceptions encountered by the check.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="taskException">Exception encountered.</param>
        protected override void HandleCheckException(MethodBase testOrigin,
            ParameterInfo testParam, AggregateException taskException)
        {
            if (testOrigin == null) throw new ArgumentNullException(nameof(testOrigin));
            if (testParam == null) throw new ArgumentNullException(nameof(testParam));
            if (taskException == null) throw new ArgumentNullException(nameof(taskException));

            Exception actual = taskException.InnerExceptions.Single();
            if (actual is TargetInvocationException ex)
            {
                actual = ex.InnerException;
            }

            string details = $"on method '{testOrigin.Name}' with parameter '{testParam.Name}'";

            if (actual is ArgumentNullException inner
                && testOrigin.Name == inner.TargetSite.Name)
            {
                m_Asserter.Is(testParam.Name, inner.ParamName,
                    $"Incorrect name provided for exception {details}.");
            }
            else
            {
                m_Asserter.Is(false, actual is NullReferenceException,
                    $"Null reference exception encountered {details}.");
            }
        }
    }
}
