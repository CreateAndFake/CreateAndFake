using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of injected dummies for the randomizer.</summary>
    public sealed class InjectedCreateHint : CreateHint
    {
        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            if (type.Inherits(typeof(Injected<>)))
            {
                return (true, Create(type, randomizer));
            }
            else
            {
                return (false, null);
            }
        }

        /// <summary>Creates a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>Created instance.</returns>
        private static object Create(Type type, RandomizerChainer randomizer)
        {
            Type target = type.GetGenericArguments().Single();

            // Finds the contructor with the most class references then by fewest parameters.
            ConstructorInfo maker = target.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .GroupBy(c => c.GetParameters().Count(p => randomizer.FakerSupports(p.ParameterType)))
                .OrderByDescending(g => g.Key)
                .FirstOrDefault()
                ?.OrderBy(c => c.GetParameters())
                .FirstOrDefault();

            if (maker != null)
            {
                ParameterInfo[] info = maker.GetParameters();
                object[] args = new object[info.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    if (randomizer.FakerSupports(info[i].ParameterType))
                    {
                        args[i] = randomizer.Create(typeof(Fake<>).MakeGenericType(info[i].ParameterType));
                    }
                    else
                    {
                        args[i] = randomizer.Create(info[i].ParameterType);
                    }
                }

                return type
                    .GetConstructor(new[] { target, typeof(IEnumerable<Fake>) })
                    .Invoke(new[]
                    {
                        maker.Invoke(args.Select(v => (v is Fake fake) ? fake.Dummy : v).ToArray()),
                        args.OfType<Fake>()
                    });
            }
            else
            {
                throw new InvalidOperationException($"No constructors found on type '{target}'.");
            }
        }
    }
}
