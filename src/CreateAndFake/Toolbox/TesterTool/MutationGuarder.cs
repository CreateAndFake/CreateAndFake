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
        private readonly IDuplicator m_Duplicator;

        /// <summary>Handles common test scenarios.</summary>
        private readonly Asserter m_Asserter;

        /// <summary>Sets up the guarder capabilities.</summary>
        /// <param name="fixer">Handles generic resolution.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        /// <param name="duplicator">Deep clones objects.</param>
        /// <param name="asserter">Handles common test scenarios.</param>
        /// <param name="timeout">How long to wait for methods to complete.</param>
        internal MutationGuarder(GenericFixer fixer, IRandomizer randomizer,
            IDuplicator duplicator, Asserter asserter, TimeSpan timeout)
            : base(fixer, randomizer, timeout)
        {
            m_Duplicator = duplicator ?? throw new ArgumentNullException(nameof(duplicator));
            m_Asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
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
                .Where(m => m.Name != "Finalize" && m.Name != "Dispose")
                .Where(m => !m.IsPrivate))
            {
                PreventsMutation(instance, Fixer.FixMethod(method), false);
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
                PreventsMutation(null, Fixer.FixMethod(method),
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
                .Select(p => (!p.ParameterType.IsByRef) ? Randomizer.Create(p.ParameterType) : null)
                .ToArray();
            object[] copy = m_Duplicator.Copy(data);

            object result;
            if (instance == null && method is ConstructorInfo builder)
            {
                result = RunCheck(method, null, () => builder.Invoke(data));
            }
            else
            {
                result = RunCheck(method, null, () => method.Invoke(instance, data));
            }

            if (result != null && callAllMethods)
            {
                CallAllMethods(method, null, result);
            }
            (result as IDisposable)?.Dispose();

            m_Asserter.ValuesEqual(copy, data, $"Parameter data was mutated when testing '{method.Name}'.");
        }

        /// <summary>Handles exceptions encountered by the check.</summary>
        /// <param name="testOrigin">Method under test.</param>
        /// <param name="testParam">Parameter being set to null.</param>
        /// <param name="taskException">Exception encountered.</param>
        protected override void HandleCheckException(MethodBase testOrigin,
            ParameterInfo testParam, AggregateException taskException)
        { }
    }
}
