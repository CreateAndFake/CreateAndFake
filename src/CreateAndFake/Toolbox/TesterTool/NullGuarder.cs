using System;
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
        /// <summary>Core value random handler.</summary>
        private readonly IRandom m_Gen;

        /// <summary>Creates objects and populates them with random values.</summary>
        private readonly IRandomizer m_Randomizer;

        /// <summary>Handles common test scenarios.</summary>
        private readonly Asserter m_Asserter;

        /// <summary>Sets up the guarder capabilities.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        internal NullGuarder(IRandom gen, IRandomizer randomizer, Asserter asserter)
        {
            m_Gen = gen ?? throw new ArgumentNullException(nameof(gen));
            m_Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            m_Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
        }

        /// <summary>Verifies nulls are guarded on the type.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        /// <remarks>
        ///     Tests each parameter possible with null.
        ///     Constructor parameters are additionally tested by running all methods.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </remarks>
        internal void PreventsNullRefException<T>()
        {
            foreach (ConstructorInfo constructor in typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => c.IsPublic || c.IsAssembly || c.IsFamilyOrAssembly))
            {
                PreventsNullRefException(null, constructor);
            }

            if (!(typeof(T).IsAbstract && typeof(T).IsSealed))
            {
                T instance = m_Randomizer.Create<T>();
                PreventsNullRefException(instance);
                (instance as IDisposable)?.Dispose();
            }
            else
            {
                foreach (MethodInfo method in typeof(T)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.IsPublic || m.IsAssembly))
                {
                    PreventsNullRefException(null, FixGenerics(method));
                }
            }
        }

        /// <summary>Verifies nulls are guarded on methods.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        /// <param name="instance">Instance to test the methods on.</param>
        /// <remarks>
        ///     Tests each parameter possible with null.
        ///     Static methods are also tested on the type.
        ///     Ignores any exception besides NullReferenceException and moves on.
        /// </remarks>
        internal void PreventsNullRefException<T>(T instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            if (!(typeof(T).IsAbstract && typeof(T).IsSealed))
            {
                foreach (MethodInfo method in typeof(T)
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.IsPublic || m.IsAssembly || m.IsFamilyOrAssembly))
                {
                    PreventsNullRefException(instance, FixGenerics(method));
                }
            }

            foreach (MethodInfo method in typeof(T)
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly))
            {
                PreventsNullRefException(null, FixGenerics(method));
            }
        }

        /// <summary>Verifies nulls are guarded on a method.</summary>
        /// <param name="instance">Instance with the method under test.</param>
        /// <param name="method">Method under test.</param>
        private void PreventsNullRefException(object instance, MethodBase method)
        {
            object[] data = method.GetParameters()
                .Select(p => m_Randomizer.Create(p.ParameterType)).ToArray();

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
                    if (result != null)
                    {
                        CallAllMethods(method, param, result);
                    }
                }
                else
                {
                    result = NullCheck(method, param, () => method.Invoke(instance, data));
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
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamilyOrAssembly)
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
                Task<object> task = Task.Run(() => call.Invoke());
                if (!task.Wait(new TimeSpan(0, 0, 2)))
                {
                    throw new TimeoutException($"Attempting to run method '{testOrigin.Name}' timed out.");
                }
                return task.Result;
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions[0] is TargetInvocationException ex)
                {
                    string details = $"on method '{testOrigin.Name}' with parameter '{testParam.Name}'";

                    if (e.InnerException is ArgumentNullException inner
                        && testOrigin.Name == inner.TargetSite.Name)
                    {
                        m_Asserter.Is(testParam.Name, inner.ParamName,
                            $"Incorrect name provided for exception {details}.");
                    }
                    else if (e.InnerException is NullReferenceException)
                    {
                        m_Asserter.Fail(e.InnerException,
                            $"Null reference exception encountered {details}.");
                    }
                    return null;
                }
                else
                {
                    throw;
                }
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
