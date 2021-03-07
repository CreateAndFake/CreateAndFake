using System;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Automates parameter mutation checks.</summary>
    internal sealed class MutationGuarder : BaseGuarder
    {
        /// <summary>Deep clones objects.</summary>
        private readonly IDuplicator _duplicator;

        /// <summary>Handles common test scenarios.</summary>
        private readonly Asserter _asserter;

        /// <summary>Initializes a new instance of the <see cref="MutationGuarder"/> class.</summary>
        /// <param name="fixer">Handles generic resolution.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="duplicator">Deep clones objects.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <param name="timeout">How long to wait for methods to complete.</param>
        internal MutationGuarder(GenericFixer fixer, IRandomizer randomizer,
            IDuplicator duplicator, Asserter asserter, TimeSpan timeout)
            : base(fixer, randomizer, timeout)
        {
            _duplicator = duplicator ?? throw new ArgumentNullException(nameof(duplicator));
            _asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
        }

        /// <summary>Verifies mutations are prevented on constructors.</summary>
        /// <param name="type">Type to verify.</param>
        /// <param name="callAllMethods">Run instance methods to validate constructor parameters.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        internal void PreventsMutationOnConstructors(Type type, bool callAllMethods, params object[] injectionValues)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (ConstructorInfo constructor in FindAllConstructors(type))
            {
                PreventsMutation(null, constructor, callAllMethods, injectionValues);
            }
        }

        /// <summary>Verifies mutations are prevented on methods.</summary>
        /// <param name="instance">Instance to test the methods on.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        internal void PreventsMutationOnMethods(object instance, params object[] injectionValues)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            foreach (MethodInfo method in FindAllMethods(instance.GetType(), BindingFlags.Instance)
                .Where(m => m.Name is not "Finalize" and not "Dispose"))
            {
                PreventsMutation(instance, Fixer.FixMethod(method), false, injectionValues);
            }
        }

        /// <summary>Verifies mutations are prevented on methods.</summary>
        /// <param name="type">Type to verify.</param>
        /// <param name="callAllMethods">Run instance methods to validate factory parameters.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        internal void PreventsMutationOnStatics(Type type, bool callAllMethods, params object[] injectionValues)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            foreach (MethodInfo method in FindAllMethods(type, BindingFlags.Static))
            {
                PreventsMutation(null, Fixer.FixMethod(method),
                    callAllMethods && method.ReturnType.Inherits(type), injectionValues);
            }
        }

        /// <summary>Verifies mutations are prevented on a method.</summary>
        /// <param name="instance">Instance with the method under test.</param>
        /// <param name="method">Method under test.</param>
        /// <param name="callAllMethods">If all instance methods should be called after the method.</param>
        /// <param name="injectionValues">Values to inject into the method.</param>
        private void PreventsMutation(object instance,
            MethodBase method, bool callAllMethods, object[] injectionValues)
        {
            object[] data = null;
            object[] copy = null;
            object result = null;
            try
            {
                data = Randomizer.CreateFor(method, injectionValues).Args.ToArray();
                copy = _duplicator.Copy(data);

                result = (instance == null && method is ConstructorInfo builder)
                    ? RunCheck(method, null, () => builder.Invoke(data))
                    : RunCheck(method, null, () => method.Invoke(instance, data));

                if (result != null && callAllMethods)
                {
                    CallAllMethods(method, null, result, injectionValues);
                }

                _asserter.ValuesEqual(copy, data, $"Parameter data was mutated when testing '{method.Name}'.");
            }
            finally
            {
                DisposeAllButInjected(injectionValues, data, copy, result);
            }
        }

        /// <summary>Handles exceptions encountered by the check.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="taskException">Exception encountered.</param>
        protected override void HandleCheckException(MethodBase testOrigin,
            ParameterInfo testParam, Exception taskException)
        { }
    }
}
