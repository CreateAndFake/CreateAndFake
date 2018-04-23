using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Manages faking objects.</summary>
    /// <typeparam name="T">Type being faked.</typeparam>
    [SuppressMessage("Sonar", "S3242:ConsiderGeneralType", Justification = "Provides type safety.")]
    public sealed class Fake<T> : Fake
    {
        /// <summary>Faked implementation.</summary>
        public new T Dummy => (T)base.Dummy;

        /// <summary>Sets up the fake harness.</summary>
        /// <param name="fake">Faked implementation.</param>
        /// <param name="valuer">How to compare call data.</param>
        public Fake(IFaked fake, IValuer valuer = null) : base(fake, valuer) { }

        /// <summary>Switches the fake to a different type.</summary>
        /// <param name="baseFake">Created fake with the extra type.</param>
        [SuppressMessage("Sonar", "S1481:RemoveUnusedLocals", Justification = "Required for cast check.")]
        public Fake(Fake baseFake) : base(baseFake)
        {
            T check = (T)base.Dummy;
        }

        /// <summary>Ties a set method call to fake behavior.</summary>
        /// <typeparam name="TResult">Property type.</typeparam>
        /// <param name="method">Expression of property to setup.</param>
        /// <param name="value">Set value to match from the call.</param>
        /// <param name="callback">Fake behavior to invoke.</param>
        /// <returns>Representation of the call.</returns>
        public void SetupSet<TResult>(Expression<Func<T, TResult>> method, TResult value, Behavior<VoidType> callback)
        {
            (MethodInfo, Type[], object[]) call = ExtractCall(method, true);
            Setup(call.Item1.Name, call.Item2, new object[] { value }, callback);
        }

        /// <summary>Ties a set method call to fake behavior.</summary>
        /// <typeparam name="TResult">Property type.</typeparam>
        /// <param name="method">Expression of property to setup.</param>
        /// <param name="value">Arg expression to match from the call.</param>
        /// <param name="callback">Fake behavior to invoke.</param>
        /// <returns>Representation of the call.</returns>
        public void SetupSet<TResult>(Expression<Func<T, TResult>> method, Arg value, Behavior<VoidType> callback)
        {
            (MethodInfo, Type[], object[]) call = ExtractCall(method, true);
            Setup(call.Item1.Name, call.Item2, new object[] { value }, callback);
        }

        /// <summary>Ties a method call to fake behavior.</summary>
        /// <param name="method">Expression of method to setup.</param>
        /// <param name="callback">Fake behavior to invoke.</param>
        /// <returns>Representation of the call.</returns>
        public void Setup(Expression<Action<T>> method, Behavior<VoidType> callback)
        {
            (MethodInfo, Type[], object[]) call = ExtractCall(method, false);
            Setup(call.Item1.Name, call.Item2, call.Item3, callback);
        }

        /// <summary>Ties a method call to fake behavior.</summary>
        /// <typeparam name="TResult">Method return type.</typeparam>
        /// <param name="method">Expression of method to setup.</param>
        /// <param name="callback">Fake behavior to invoke.</param>
        /// <returns>Representation of the call.</returns>
        public void Setup<TResult>(Expression<Func<T, TResult>> method, Behavior<TResult> callback)
        {
            (MethodInfo, Type[], object[]) call = ExtractCall(method, false);
            Setup(call.Item1.Name, call.Item2, call.Item3, callback);
        }

        /// <summary>Verifies the number of calls made to the setter.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="method">Setter to verify.</param>
        /// <param name="value">Set value to match from the call.</param>
        public void VerifySet<TResult>(Times times, Expression<Func<T, TResult>> method, TResult value)
        {
            (MethodInfo, Type[], object[]) call = ExtractCall(method, true);
            Verify(times, call.Item1.Name, call.Item2, new object[] { value });
        }

        /// <summary>Verifies the number of calls made to the setter.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="method">Setter to verify.</param>
        /// <param name="value">Arg expression to match from the call.</param>
        public void VerifySet<TResult>(Times times, Expression<Func<T, TResult>> method, Arg value)
        {
            (MethodInfo, Type[], object[]) call = ExtractCall(method, true);
            Verify(times, call.Item1.Name, call.Item2, new object[] { value });
        }

        /// <summary>Verifies the number of calls made to the method.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="method">Method to verify.</param>
        public void Verify(Times times, Expression<Action<T>> method)
        {
            (MethodInfo, Type[], object[]) call = ExtractCall(method, false);
            Verify(times, call.Item1.Name, call.Item2, call.Item3);
        }

        /// <summary>Verifies the number of calls made to the method.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="method">Method to verify.</param>
        public void Verify<TResult>(Times times, Expression<Func<T, TResult>> method)
        {
            (MethodInfo, Type[], object[]) call = ExtractCall(method, false);
            Verify(times, call.Item1.Name, call.Item2, call.Item3);
        }

        /// <summary>Changes expression to call data.</summary>
        /// <param name="method">Expression to convert.</param>
        /// <param name="onlySetter">If only setter is allowed.</param>
        /// <returns>Method name, generics, and args.</returns>
        private static (MethodInfo, Type[], object[]) ExtractCall(LambdaExpression method, bool onlySetter)
        {
            if (!onlySetter && method.Body is MethodCallExpression methodCall)
            {
                Type[] generics = (methodCall.Method.IsGenericMethod
                    ? methodCall.Method.GetGenericArguments()
                    : Type.EmptyTypes);

                return (methodCall.Method, generics, methodCall.Arguments.Select(a => ConvertArg(a)).ToArray());
            }
            else if (method.Body is MemberExpression memberExpression)
            {
                PropertyInfo info = (PropertyInfo)memberExpression.Member;
                if (onlySetter)
                {
                    return (info.GetSetMethod(), Type.EmptyTypes, Array.Empty<object>());
                }
                else
                {
                    return (info.GetGetMethod(), Type.EmptyTypes, Array.Empty<object>());
                }
            }
            else
            {
                throw new InvalidOperationException($"Unexpected expression type: {method}");
            }
        }

        /// <summary>Converts arg expressions to actual values.</summary>
        /// <param name="arg">Arg to convert.</param>
        /// <returns>Value to pass to the call.</returns>
        private static object ConvertArg(Expression arg)
        {
            if (arg is MemberExpression memberExpression
                && memberExpression.Member.Name == nameof(OutRef<Type>.Var))
            {
                return ConvertArg(memberExpression.Expression);
            }

            MethodCallExpression call = arg as MethodCallExpression
                ?? (arg as UnaryExpression)?.Operand as MethodCallExpression;
            if (call?.Method.DeclaringType == typeof(Arg) && call.Method.ReturnType != typeof(Arg))
            {
                Type innerType = (call.Method.ReturnType.AsGenericType() == typeof(OutRef<>))
                    ? call.Method.ReturnType.GetGenericArguments().Single()
                    : call.Method.ReturnType;

                return typeof(Arg)
                    .GetMethod("Lambda" + call.Method.Name)
                    .MakeGenericMethod(innerType)
                    .Invoke(null, call.Arguments.Select(a => ConvertArg(a)).ToArray());
            }
            else
            {
                return Expression.Lambda(arg).Compile().DynamicInvoke();
            }
        }
    }
}
