using System;
using System.Linq;
using System.Reflection;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints
{
    /// <summary>Handles generation of exceptions.</summary>
    public sealed class ExceptionCreateHint : CreateHint
    {
        /// <summary>Tries to create a random instance of the given type.</summary>
        /// <param name="type">Type to generate.</param>
        /// <param name="randomizer">Handles callback behavior for child values.</param>
        /// <returns>If the type could be created and the created instance.</returns>
        protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (randomizer == null) throw new ArgumentNullException(nameof(randomizer));

            if (!type.Inherits<Exception>())
            {
                return (false, null);
            }

            ConstructorInfo[] options = type.FindLocalSubclasses()
                .Where(t => t.IsVisible)
                .Where(t => t.IsSerializable)
#if NETSTANDARD // Security exceptions don't work with default serialization in .NET full.
                .Where(t => !t.Namespace.StartsWith("System.Security", StringComparison.Ordinal))
#endif
                .Select(t => t.GetConstructor(new[] { typeof(string) }))
                .Where(c => c != null)
                .ToArray();

            if (options.Any())
            {
                return (true, randomizer.Gen.NextItem(options).Invoke(new[] { randomizer.Create<string>() }));
            }
            else
            {
                return (false, null);
            }
        }
    }
}
