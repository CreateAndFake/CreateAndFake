using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.FakerTool;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles generation of injected dummies for the randomizer.</summary>
public sealed class InjectedCreateHint : CreateHint
{
    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

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

        ConstructorInfo maker = FindConstructor(target, randomizer, BindingFlags.Public)
            ?? FindConstructor(target, randomizer, BindingFlags.NonPublic);

        if (maker != null)
        {
            ParameterInfo[] info = maker.GetParameters();
            object[] args = new object[info.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = randomizer.FakerSupports(info[i].ParameterType)
                    ? randomizer.Create(typeof(Fake<>).MakeGenericType(info[i].ParameterType))
                    : randomizer.Create(info[i].ParameterType);
            }

            return type
                .GetConstructor([target, typeof(IEnumerable<Fake>)])
                .Invoke(
                [
                    maker.Invoke(args.Select(v => (v is Fake fake) ? fake.Dummy : v).ToArray()),
                    args.OfType<Fake>()
                ]);
        }
        else
        {
            throw new InvalidOperationException($"No constructors found on type '{target}'.");
        }
    }

    /// <summary>Finds the constructor with the most class references then by fewest parameters.</summary>
    /// <param name="target">Type to find a constructor for.</param>
    /// <param name="randomizer">Handles callback behavior for child values.</param>
    /// <param name="scope">Scope of constructors to find.</param>
    /// <returns>Constructor if found; null otherwise.</returns>
    private static ConstructorInfo FindConstructor(Type target, RandomizerChainer randomizer, BindingFlags scope)
    {
        return target.GetConstructors(BindingFlags.Instance | scope)
            .GroupBy(c => c.GetParameters().Count(p => randomizer.FakerSupports(p.ParameterType)))
            .OrderByDescending(g => g.Key)
            .FirstOrDefault()
            ?.OrderBy(c => c.GetParameters())
            .FirstOrDefault();
    }
}
