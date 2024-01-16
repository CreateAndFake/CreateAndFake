using System;
using System.Linq;
using System.Threading.Tasks;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.RandomizerTool.CreateHints;

/// <summary>Handles generation of injected dummies for the randomizer.</summary>
public sealed class TaskCreateHint : CreateHint
{
    /// <inheritdoc/>
    protected internal override (bool, object) TryCreate(Type type, RandomizerChainer randomizer)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));
        ArgumentGuard.ThrowIfNull(randomizer, nameof(randomizer));

        if (type.Inherits<Task>() || typeof(TaskCompletionSource<>).IsInheritedBy(type))
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
    /// <returns>The created instance.</returns>
    private static object Create(Type type, RandomizerChainer randomizer)
    {
        if (type.IsGenericType)
        {
            Type content = type.GetGenericArguments().Single();

            return typeof(Task)
                .GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(content)
                .Invoke(null, [randomizer.Create(content, randomizer.Parent)]);
        }
        else
        {
            return Task.FromResult(randomizer.Create<int>());
        }
    }
}
