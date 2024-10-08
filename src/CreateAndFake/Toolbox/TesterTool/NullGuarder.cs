﻿using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.RandomizerTool;

namespace CreateAndFake.Toolbox.TesterTool;

/// <summary>Automates null reference guard checks.</summary>
internal sealed class NullGuarder : BaseGuarder
{
    /// <summary>Handles common test scenarios.</summary>
    private readonly Asserter _asserter;

    /// <summary>Initializes a new instance of the <see cref="NullGuarder"/> class.</summary>
    /// <param name="fixer">Handles generic resolution.</param>
    /// <param name="randomizer">Creates objects and populates them with random values.</param>
    /// <param name="asserter">Handles common test scenarios.</param>
    /// <param name="timeout">How long to wait for methods to complete.</param>
    internal NullGuarder(GenericFixer fixer, IRandomizer randomizer, Asserter asserter, TimeSpan timeout)
        : base(fixer, randomizer, timeout)
    {
        _asserter = asserter ?? throw new ArgumentNullException(nameof(asserter));
    }

    /// <summary>
    ///     Verifies nulls are guarded on constructors.
    ///     Tests each nullable parameter possible with null.
    ///     Ignores any exception besides NullReferenceException and moves on.
    /// </summary>
    /// <param name="type">Type to verify.</param>
    /// <param name="callAllMethods">Run instance methods to validate constructor parameters.</param>
    /// <param name="injectionValues">Values to inject into the method.</param>
    internal void PreventsNullRefExceptionOnConstructors(
        Type type, bool callAllMethods, params object[] injectionValues)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        foreach (ConstructorInfo constructor in FindAllConstructors(type))
        {
            PreventsNullRefException(null, constructor, callAllMethods, injectionValues);
        }
    }

    /// <summary>
    ///     Verifies nulls are guarded on methods.
    ///     Tests each nullable parameter possible with null.
    ///     Ignores any exception besides NullReferenceException and moves on.
    /// </summary>
    /// <param name="instance">Instance to test the methods on.</param>
    /// <param name="injectionValues">Values to inject into the method.</param>
    internal void PreventsNullRefExceptionOnMethods(object instance, params object?[]? injectionValues)
    {
        ArgumentGuard.ThrowIfNull(instance, nameof(instance));

        foreach (MethodInfo method in FindAllMethods(instance.GetType(), BindingFlags.Instance)
            .Where(m => m.Name is not "Finalize" and not "Dispose"))
        {
            PreventsNullRefException(instance, Fixer.FixMethod(method), false, injectionValues);
        }
    }

    /// <summary>
    ///     Verifies nulls are guarded on methods.
    ///     Tests each nullable parameter possible with null.
    ///     Ignores any exception besides NullReferenceException and moves on.
    /// </summary>
    /// <param name="type">Type to verify.</param>
    /// <param name="callAllMethods">Run instance methods to validate factory parameters.</param>
    /// <param name="injectionValues">Values to inject into the method.</param>
    internal void PreventsNullRefExceptionOnStatics(
        Type type, bool callAllMethods, params object[] injectionValues)
    {
        ArgumentGuard.ThrowIfNull(type, nameof(type));

        foreach (MethodInfo method in FindAllMethods(type, BindingFlags.Static))
        {
            PreventsNullRefException(null, Fixer.FixMethod(method),
                callAllMethods && method.ReturnType.Inherits(type), injectionValues);
        }
    }

    /// <summary>Verifies nulls are guarded on a method.</summary>
    /// <param name="instance">Instance with the method under test.</param>
    /// <param name="method">Method under test.</param>
    /// <param name="callAllMethods">If all instance methods should be called after the method.</param>
    /// <param name="injectionValues">Values to inject into the method.</param>
    private void PreventsNullRefException(object? instance,
        MethodBase method, bool callAllMethods, object?[]? injectionValues)
    {
        object?[]? data = null;
        object? result = null;
        try
        {
            data = Randomizer.CreateFor(method, injectionValues).Args.ToArray();

            for (int i = 0; i < data.Length; i++)
            {
                ParameterInfo param = method.GetParameters()[i];
                if (param.ParameterType.IsValueType
                    && Nullable.GetUnderlyingType(param.ParameterType) == null)
                {
                    continue;
                }

                object? original = data[i];
                data[i] = null;
                try
                {
                    result = (instance == null && method is ConstructorInfo builder)
                        ? RunCheck(method, param, () => builder.Invoke(data))
                        : RunCheck(method, param, () => method.Invoke(instance, data)!);

                    if (result != null && callAllMethods)
                    {
                        CallAllMethods(method, param, result, injectionValues);
                    }
                }
                finally
                {
                    data[i] = original;
                }
            }
        }
        finally
        {
            DisposeAllButInjected(injectionValues, data, result);
        }
    }

    /// <inheritdoc/>
    protected override void HandleCheckException(MethodBase testOrigin,
        ParameterInfo? testParam, Exception taskException)
    {
        ArgumentGuard.ThrowIfNull(testOrigin, nameof(testOrigin));
        ArgumentGuard.ThrowIfNull(testParam, nameof(testParam));
        ArgumentGuard.ThrowIfNull(taskException, nameof(taskException));

        string details = $"on method '{testOrigin.Name}' with parameter '{testParam.Name}'";

        _asserter.Is(false, taskException is NullReferenceException,
            $"Null reference exception encountered {details}.");
    }
}
