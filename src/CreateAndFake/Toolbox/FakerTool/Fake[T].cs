﻿using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.FakerTool.Proxy;

namespace CreateAndFake.Toolbox.FakerTool;

/// <typeparam name="T"><c>Type</c> being faked.</typeparam>
/// <inheritdoc/>
public sealed class Fake<T> : Fake
{
    /// <inheritdoc cref="Fake.Dummy"/>
    public new T Dummy => (T)base.Dummy;

    /// <inheritdoc/>
    public Fake(IFaked fake) : base(fake) { }

    /// <inheritdoc/>
    /// <remarks>Switches the fake to a different type.</remarks>
    public Fake(Fake baseFake) : base(baseFake?.Dummy ?? throw new ArgumentNullException(nameof(baseFake)))
    {
        _ = (T)base.Dummy;
    }

    /// <summary>Ties a set method call to fake behavior.</summary>
    /// <typeparam name="TResult">Property type.</typeparam>
    /// <param name="method">Expression of property to setup.</param>
    /// <param name="value">Set value to match from the call.</param>
    /// <param name="callback">Fake behavior to invoke.</param>
    /// <returns>Representation of the call.</returns>
    public void SetupSet<TResult>(Expression<Func<T, TResult>> method, TResult value, Behavior<VoidType> callback)
    {
        (MethodInfo, Type[], object?[]) call = ExtractCall(method, true);
        Setup(call.Item1.Name, call.Item2, [value], callback);
    }

    /// <summary>Ties a set method call to fake behavior.</summary>
    /// <typeparam name="TResult">Property type.</typeparam>
    /// <param name="method">Expression of property to setup.</param>
    /// <param name="value">Arg expression to match from the call.</param>
    /// <param name="callback">Fake behavior to invoke.</param>
    /// <returns>Representation of the call.</returns>
    public void SetupSet<TResult>(Expression<Func<T, TResult>> method, Arg value, Behavior<VoidType> callback)
    {
        (MethodInfo, Type[], object?[]) call = ExtractCall(method, true);
        Setup(call.Item1.Name, call.Item2, [value], callback);
    }

    /// <summary>Ties a method call to fake behavior.</summary>
    /// <param name="method">Expression of method to setup.</param>
    /// <param name="callback">Fake behavior to invoke.</param>
    /// <returns>Representation of the call.</returns>
    public void Setup(Expression<Action<T>> method, Behavior<VoidType> callback)
    {
        (MethodInfo, Type[], object?[]) call = ExtractCall(method, false);
        Setup(call.Item1.Name, call.Item2, call.Item3, callback);
    }

    /// <summary>Ties a method call to fake behavior.</summary>
    /// <typeparam name="TResult">Method return type.</typeparam>
    /// <param name="method">Expression of method to setup.</param>
    /// <param name="callback">Fake behavior to invoke.</param>
    /// <returns>Representation of the call.</returns>
    public void Setup<TResult>(Expression<Func<T, TResult>> method, Behavior<TResult> callback)
    {
        (MethodInfo, Type[], object?[]) call = ExtractCall(method, false);
        Setup(call.Item1.Name, call.Item2, call.Item3, callback);
    }

    /// <summary>Verifies the number of calls made to the setter.</summary>
    /// <param name="times">Expected number of calls.</param>
    /// <param name="method">Setter to verify.</param>
    /// <param name="value">Set value to match from the call.</param>
    public void VerifySet<TResult>(Times times, Expression<Func<T, TResult>> method, TResult value)
    {
        (MethodInfo, Type[], object?[]) call = ExtractCall(method, true);
        Verify(times, call.Item1.Name, call.Item2, [value]);
    }

    /// <summary>Verifies the number of calls made to the setter.</summary>
    /// <param name="times">Expected number of calls.</param>
    /// <param name="method">Setter to verify.</param>
    /// <param name="value">Arg expression to match from the call.</param>
    public void VerifySet<TResult>(Times times, Expression<Func<T, TResult>> method, Arg value)
    {
        (MethodInfo, Type[], object?[]) call = ExtractCall(method, true);
        Verify(times, call.Item1.Name, call.Item2, [value]);
    }

    /// <summary>Verifies the number of calls made to <paramref name="method"/>.</summary>
    /// <param name="times">Expected number of calls.</param>
    /// <param name="method">Method to verify.</param>
    public void Verify(Times times, Expression<Action<T>> method)
    {
        (MethodInfo, Type[], object?[]) call = ExtractCall(method, false);
        Verify(times, call.Item1.Name, call.Item2, call.Item3);
    }

    /// <summary>Verifies the number of calls made to <paramref name="method"/>.</summary>
    /// <param name="times">Expected number of calls.</param>
    /// <param name="method">Method to verify.</param>
    public void Verify<TResult>(Times times, Expression<Func<T, TResult>> method)
    {
        (MethodInfo, Type[], object?[]) call = ExtractCall(method, false);
        Verify(times, call.Item1.Name, call.Item2, call.Item3);
    }

    /// <summary>Changes expression to call data.</summary>
    /// <param name="method">Expression to convert.</param>
    /// <param name="onlySetter">If only setter is allowed.</param>
    /// <returns>Method name, generics, and args.</returns>
    private static (MethodInfo, Type[], object?[]) ExtractCall(LambdaExpression method, bool onlySetter)
    {
        ArgumentGuard.ThrowIfNull(method, nameof(method));

        if (!onlySetter && method.Body is MethodCallExpression methodCall)
        {
            if (methodCall.Method.IsStatic)
            {
                throw new NotSupportedException(
                    $"Method '{methodCall.Method.Name}' is static and not an actual member of '{typeof(T).Name}'.");
            }

            Type[] generics = methodCall.Method.IsGenericMethod
                ? methodCall.Method.GetGenericArguments()
                : Type.EmptyTypes;

            return (methodCall.Method, generics, methodCall.Arguments.Select(ConvertArg).ToArray());
        }
        else if (method.Body is MemberExpression memberExpression)
        {
            PropertyInfo info = (PropertyInfo)memberExpression.Member;
            return onlySetter
                ? (info.GetSetMethod()!, Type.EmptyTypes, Array.Empty<object>())
                : (info.GetGetMethod()!, Type.EmptyTypes, Array.Empty<object>());
        }
        else
        {
            throw new InvalidOperationException($"Unexpected expression type: {method}");
        }
    }

    /// <summary>Converts arg expressions to actual values.</summary>
    /// <param name="arg">Arg to convert.</param>
    /// <returns>Value to pass to the call.</returns>
    private static object? ConvertArg(Expression arg)
    {
        if (arg is MemberExpression memberExpression
            && memberExpression.Member.Name == nameof(OutRef<Type>.Var))
        {
            return ConvertArg(memberExpression.Expression!);
        }

        MethodCallExpression? call = arg as MethodCallExpression
            ?? (arg as UnaryExpression)?.Operand as MethodCallExpression;
        if (call?.Method.DeclaringType == typeof(Arg) && call.Method.ReturnType != typeof(Arg))
        {
            return ResolveArgLambda(call);
        }
        else
        {
            return Expression.Lambda(arg).Compile().DynamicInvoke();
        }
    }

    /// <summary>Converts an lambda arg expression to its actual value.</summary>
    /// <param name="call">Expression of the arg to convert.</param>
    /// <returns>Value to pass to the call.</returns>
    [SuppressMessage("Microsoft.Style", "IDE0200:PreferMethodGroupConversion", Justification = "Code coverage issue.")]
    private static object ResolveArgLambda(MethodCallExpression call)
    {
        Type innerType = (call.Method.ReturnType.AsGenericType() == typeof(OutRef<>))
                ? call.Method.ReturnType.GetGenericArguments().Single()
                : call.Method.ReturnType;

        return typeof(Arg)
            .GetMethod("Lambda" + call.Method.Name)!
            .MakeGenericMethod(innerType)
            .Invoke(null, call.Arguments.Select(x => ConvertArg(x)).ToArray())!;
    }
}
