using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates common tests.</summary>
    public class Tester
    {
        /// <summary>Core value random handler.</summary>
        protected IRandom Gen { get; }

        /// <summary>Creates objects and populates them with random values.</summary>
        protected IRandomizer Randomizer { get; }

        /// <summary>Handles common test scenarios.</summary>
        protected Asserter Asserter { get; }

        /// <summary>Sets up the asserter capabilities.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <exception cref="ArgumentNullException">If given nulls.</exception>
        public Tester(IRandom gen, IRandomizer randomizer, Asserter asserter)
        {
            Gen = gen ?? throw new ArgumentNullException(nameof(gen));
            Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
        }

        /// <summary>Verifies nulls are guarded on the type.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        public virtual void PreventsNullRefException<T>()
        {
            if (!Task.Run(() => RunPreventsNullRefException<T>()).Wait(new TimeSpan(0, 0, 3)))
            {
                throw new TimeoutException();
            }
        }

        /// <summary>Verifies nulls are guarded on the type.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        private void RunPreventsNullRefException<T>()
        {
            foreach (ConstructorInfo constructor in typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => c.IsPublic || c.IsAssembly || c.IsFamilyOrAssembly))
            {
                PreventsNullRefException(null, constructor, true);
            }

            if (!(typeof(T).IsAbstract && typeof(T).IsSealed))
            {
                T instance = Randomizer.Create<T>();
                foreach (MethodInfo method in typeof(T)
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.IsPublic || m.IsAssembly || m.IsFamilyOrAssembly))
                {
                    PreventsNullRefException(instance, FixGenerics(method), false);
                }
                (instance as IDisposable)?.Dispose();
            }

            foreach (MethodInfo method in typeof(T)
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly))
            {
                PreventsNullRefException(null, FixGenerics(method), false);
            }
        }

        private MethodInfo FixGenerics(MethodInfo method)
        {
            return (method.ContainsGenericParameters)
                ? method.MakeGenericMethod(method.GetGenericArguments().Select(a => CreateArg(a)).ToArray())
                : method;
        }

        private void PreventsNullRefException(object instance, MethodBase method, bool callAllMethods)
        {
            object[] data = method.GetParameters()
                .Select(p => Randomizer.Create(p.ParameterType)).ToArray();

            for (int i = 0; i < data.Length; i++)
            {
                object original = data[i];
                data[i] = null;

                CallAndCheck(method, i, () =>
                {
                    object result;
                    if (instance == null && method is ConstructorInfo builder)
                    {
                        result = builder.Invoke(data);
                    }
                    else
                    {
                        result = method.Invoke(instance, data);
                    }
                    if (callAllMethods)
                    {
                        CallAllMethods(method, i, result);
                    }
                    (result as IDisposable)?.Dispose();
                });

                data[i] = original;
            }
        }

        private void CallAllMethods(MethodBase testOrigin, int testParam, object instance)
        {
            foreach (PropertyInfo prop in instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.CanRead))
            {
                CallAndCheck(testOrigin, testParam, () => prop.GetValue(instance));
            }

            foreach (MethodInfo method in instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsAssembly || m.IsFamilyOrAssembly)
                .Select(m => FixGenerics(m)))
            {
                CallAndCheck(testOrigin, testParam, () => (method.Invoke(instance, method.GetParameters()
                    .Select(p => Randomizer.Create(p.ParameterType)).ToArray()) as IDisposable)?.Dispose());
            }
        }

        private void CallAndCheck(MethodBase testOrigin, int testParam, Action call)
        {
            try
            {
                call.Invoke();
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is ArgumentNullException inner
                    && testOrigin.Name == inner.TargetSite.Name)
                {
                    Asserter.Is(testOrigin.GetParameters()[testParam].Name, inner.ParamName,
                        $"Incorrect name provided for exception on method '{testOrigin.Name}'.");
                }
                else if (e.InnerException is NullReferenceException)
                {
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }
            }
        }

        /// <summary>Creates a concrete arg type from the given generic arg.</summary>
        /// <param name="type">Generic arg to create.</param>
        /// <returns>Created arg type.</returns>
        private Type CreateArg(Type type)
        {
            bool newNeeded = type.GenericParameterAttributes.HasFlag(
                GenericParameterAttributes.DefaultConstructorConstraint);

            Type arg;
            if (type.GenericParameterAttributes.HasFlag(
                GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                arg = Gen.NextItem(ValueRandom.ValueTypes);
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
                    arg = Randomizer.Create(Gen.NextItem(constraints)).GetType();
                }
            }).Wait();

            return arg;
        }
    }
}
