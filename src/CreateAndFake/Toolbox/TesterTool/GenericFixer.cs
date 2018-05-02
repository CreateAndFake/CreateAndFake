using System;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool
{
    /// <summary>Handles generic resolution.</summary>
    internal sealed class GenericFixer
    {
        /// <summary>Core value random handler.</summary>
        private readonly IRandom m_Gen;

        /// <summary>Creates objects and populates them with random values.</summary>
        private readonly IRandomizer m_Randomizer;

        /// <summary>Sets up the fixer capabilities.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="randomizer">Creates objects and populates them with random values.</param>
        internal GenericFixer(IRandom gen, IRandomizer randomizer)
        {
            m_Gen = gen ?? throw new ArgumentNullException(nameof(gen));
            m_Randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
        }

        /// <summary>Defines any generics in a method.</summary>
        /// <param name="method">Method to fix.</param>
        /// <returns>Method with all generics defined.</returns>
        internal MethodInfo FixMethod(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            return (method.ContainsGenericParameters)
                ? method.MakeGenericMethod(method.GetGenericArguments().Select(a => CreateArg(a)).ToArray())
                : method;
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
