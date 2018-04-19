using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates common tests.</summary>
    public class Tester
    {
        /// <summary>Creates objects and populates them with random values.</summary>
        protected IRandomizer Randomizer { get; }

        /// <summary>Handles common test scenarios.</summary>
        protected Asserter Asserter { get; }

        /// <summary>Sets up the asserter capabilities.</summary>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <exception cref="ArgumentNullException">If given nulls.</exception>
        public Tester(IRandomizer randomizer, Asserter asserter)
        {
            Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
            Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
        }

        /// <summary>Verifies nulls are guarded on the type.</summary>
        /// <typeparam name="T">Type to verify.</typeparam>
        public virtual void PreventsNullRefException<T>()
        {
            foreach (ConstructorInfo constructor in typeof(T).GetConstructors(BindingFlags.Public))
            {
                PreventsNullRefException(null, constructor, true);
            }

            T instance = Randomizer.Create<T>();
            foreach (MethodInfo method in typeof(T)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                PreventsNullRefException(instance, method, false);
            }
            (instance as IDisposable)?.Dispose();

            foreach (MethodInfo method in typeof(T)
                .GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                PreventsNullRefException(null, method, false);
            }
        }

        private void PreventsNullRefException(object instance, MethodBase method, bool callAllMethods)
        {
            object[] data = method.GetParameters()
                .Select(p => Randomizer.Create(p.ParameterType)).ToArray();

            for (int i = 0; i < data.Length; i++)
            {
                object original = data[i];
                data[i] = null;
                try
                {
                    object result = method.Invoke(instance, data);
                    if (callAllMethods)
                    {
                        CallAllMethods(result);
                    }
                (result as IDisposable)?.Dispose();
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException is ArgumentNullException inner)
                    {
                        Asserter.Is(method.GetParameters()[i].Name, inner.ParamName);
                    }
                    else if (e.InnerException is NullReferenceException)
                    {
                        ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    }
                }
                data[i] = original;
            }
        }

        private void CallAllMethods(object instance)
        {
            foreach (PropertyInfo prop in instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                prop.GetValue(instance);
            }

            foreach (MethodInfo method in instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                (method.Invoke(instance, method.GetParameters()
                    .Select(p => Randomizer.Create(p.ParameterType)).ToArray()) as IDisposable)?.Dispose();
            }
        }
    }
}
