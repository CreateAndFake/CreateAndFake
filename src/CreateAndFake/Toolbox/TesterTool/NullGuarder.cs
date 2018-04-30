using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates null reference guard checks.</summary>
    internal sealed class NullGuarder
    {
        /// <summary>Default for how long to wait for methods to complete.</summary>
        private static readonly TimeSpan s_DefaultTimeout = new TimeSpan(0, 0, 5);

        /// <summary>Core value random handler.</summary>
        private readonly IRandom m_Gen;

        /// <summary>Creates objects and populates them with random values.</summary>
        private readonly IRandomizer m_Randomizer;

        /// <summary>Handles common test scenarios.</summary>
        private readonly Asserter m_Asserter;

        /// <summary>How long to wait for methods to complete.</summary>
        private readonly TimeSpan m_Timeout;

        /// <summary>Sets up the guarder capabilities.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <param name="timeout">How long to wait for methods to complete.</param>
        internal NullGuarder(IRandom gen, IRandomizer randomizer, Asserter asserter, TimeSpan? timeout = null)
        {
            m_Gen = gen ?? throw new ArgumentNullException(nameof(gen));
            m_Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            m_Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
            m_Timeout = timeout ?? s_DefaultTimeout;
        }

        /// <summary>Verifies nulls are guarded on constructors.</summary>
        /// <param name="type">Type to verify.</param>
        /// <param name="callAllMethods">
        ///     If all instance methods are run to validate constructor parameters.
        /// </param>
        /// <remarks>
        ///     Tests each nullable parameter possible with null.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </remarks>
        internal void PreventsNullRefExceptionOnConstructors(Type type, bool callAllMethods)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (ConstructorInfo constructor in type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => c.IsPublic || c.IsAssembly || c.IsFamily || c.IsFamilyOrAssembly)
                .Where(c => !c.IsPrivate))
            {
                PreventsNullRefException(null, constructor, callAllMethods);
            }
        }

        /// <summary>Verifies nulls are guarded on methods.</summary>
        /// <param name="instance">Instance to test the methods on.</param>
        /// <remarks>
        ///     Tests each nullable parameter possible with null.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </remarks>
        internal void PreventsNullRefExceptionOnMethods(object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            foreach (MethodInfo method in instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => !m.IsPrivate))
            {
                PreventsNullRefException(instance, FixGenerics(method), false);
            }
        }

        /// <summary>Verifies nulls are guarded on methods.</summary>
        /// <param name="type">Type to verify.</param>
        /// <param name="callAllMethods">
        ///     If all instance methods are run to validate factory parameters.
        /// </param>
        /// <remarks>
        ///     Tests each nullable parameter possible with null.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </remarks>
        internal void PreventsNullRefExceptionOnStatics(Type type, bool callAllMethods)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (MethodInfo method in type
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => !m.IsPrivate))
            {
                PreventsNullRefException(null, FixGenerics(method),
                    callAllMethods && method.ReturnType.Inherits(type));
            }
        }

        /// <summary>Verifies nulls are guarded on a method.</summary>
        /// <param name="instance">Instance with the method under test.</param>
        /// <param name="method">Method under test.</param>
        /// <param name="callAllMethods">If all instance methods should be called after the method.</param>
        private void PreventsNullRefException(object instance, MethodBase method, bool callAllMethods)
        {
            object[] data = method.GetParameters()
                .Select(p => (!p.ParameterType.IsByRef) ? m_Randomizer.Create(p.ParameterType) : null)
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
                    result = NullCheck(method, param, () => builder.Invoke(data));
                }
                else
                {
                    result = NullCheck(method, param, () => method.Invoke(instance, data));
                }

                if (result != null && callAllMethods)
                {
                    CallAllMethods(method, param, result);
                }
                (result as IDisposable)?.Dispose();

                data[i] = original;
            }
        }

        /// <summary>Calls all methods to test constructor parameter being set to null.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="instance">Instance whose methods to test.</param>
        private void CallAllMethods(MethodBase testOrigin, ParameterInfo testParam, object instance)
        {
            foreach (MethodInfo method in instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamily || m.IsFamilyOrAssembly)
                .Where(m => !m.IsPrivate)
                .Select(m => FixGenerics(m)))
            {
                (NullCheck(testOrigin, testParam, () => method.Invoke(instance, method.GetParameters()
                    .Select(p => m_Randomizer.Create(p.ParameterType)).ToArray())) as IDisposable)?.Dispose();
            }
        }

        /// <summary>Tests the call prevents null reference exceptions.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="call">Call to invoke and test.</param>
        /// <returns>Returned result from the call.</returns>
        private object NullCheck(MethodBase testOrigin, ParameterInfo testParam, Func<object> call)
        {
            try
            {
                Task<object> task = Task.Run(() =>
                {
                    object result = call.Invoke();
                    if (result is IEnumerable collection)
                    {
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
            catch (AggregateException taskException)
            {
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
                else if (actual is NullReferenceException)
                {
                    m_Asserter.Fail(actual, $"Null reference exception encountered {details}.");
                }
                return null;
            }
        }

        /// <summary>Defines any generics in a method.</summary>
        /// <param name="method">Method to fix.</param>
        /// <returns>Method with all generics defined.</returns>
        private MethodInfo FixGenerics(MethodInfo method)
        {
            return (method.ContainsGenericParameters)
                ? method.MakeGenericMethod(method.GetGenericArguments().Select(a => CreateArg(a)).ToArray())
                : method;
        }

        /// <summary>Creates a concrete arg type from the given generic arg.</summary>
        /// <param name="type">Generic arg to create.</param>
        /// <returns>Created arg type.</returns>
        private Type CreateArg(Type type)
        {
            if (type.IsByRef) return null;

            bool newNeeded = type.GenericParameterAttributes.HasFlag(
                GenericParameterAttributes.DefaultConstructorConstraint);

            Type arg;
            if (type.GenericParameterAttributes.HasFlag(
                GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                arg = m_Gen.NextItem(ValueRandom.ValueTypes);
            }
            else if (newNeeded)
            {
                arg = typeof(object);
            }
            else
            {
                arg = typeof(string);
            }

            Type[] constraints = type.GetGenericParameterConstraints();

            Limiter.Dozen.Repeat(() =>
            {
                while (!constraints.All(c => arg.Inherits(c))
                    && (!newNeeded || arg.GetConstructor(Type.EmptyTypes) != null))
                {
                    arg = m_Randomizer.Create(m_Gen.NextItem(constraints)).GetType();
                }
            }).Wait();

            return arg;
        }
    }
}
