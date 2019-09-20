using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.ExceptionServices;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Mock behavior for fakes.</summary>
    [SuppressMessage("Sonar", "S2436:ReduceGenericParameters", Justification = "Must match any number.")]
    [SuppressMessage("Sonar", "S3242:ConsiderGeneralType", Justification = "Delegate doesn't match lambdas properly.")]
    public abstract class Behavior : IDeepCloneable
    {
        /// <summary>Set behavior to run.</summary>
        protected Delegate Implementation { get; }

        /// <summary>Behavior call limit.</summary>
        protected Times Limit { get; }

        /// <summary>Times the fake behavior was called.</summary>
        protected internal int Calls { get; private set; }

        /// <summary>Sets up the behavior.</summary>
        /// <param name="implementation">Set behavior to run.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <param name="calls">Starting number of calls.</param>
        protected Behavior(Delegate implementation, Times times, int calls)
        {
            Implementation = implementation;
            Limit = times;
            Calls = calls;
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <returns>Clone that is equal in value to the current instance.</returns>
        public abstract IDeepCloneable DeepClone();

        /// <summary>Runs the behavior.</summary>
        /// <param name="args">Expected args for the behavior.</param>
        /// <returns>Result of the call.</returns>
        internal object Invoke(object[] args)
        {
            Calls++;
            try
            {
                if (Implementation.Method.GetParameters().Length == 0)
                {
                    return Implementation.DynamicInvoke();
                }
                else
                {
                    return Implementation.DynamicInvoke(args);
                }
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo info = (e is TargetInvocationException)
                    ? ExceptionDispatchInfo.Capture(e.InnerException)
                    : null;

                info?.Throw();
                throw;
            }
        }

        /// <summary>Checks if call count is in expected range.</summary>
        /// <returns>True if in range; false otherwise.</returns>
        internal bool HasExpectedCalls()
        {
            return Limit?.IsInRange(Calls) ?? true;
        }

        /// <returns>String representation of expected calls.</returns>
        internal string ToExpectedCalls()
        {
            return Limit?.ToString();
        }

        /// <summary>Specifies no behavior for a fake.</summary>
        /// <param name="times">Behavior call limit.</param>
        public static Behavior<VoidType> None(Times times = null)
        {
            return Set(() => { }, times);
        }

        /// <summary>Specifies exception behavior for a fake.</summary>
        /// <param name="times">Behavior call limit.</param>
        public static Behavior<VoidType> Error(Times times = null)
        {
            return Throw<NotImplementedException>(times);
        }

        /// <summary>Specifies behavior returning null for a fake.</summary>
        /// <typeparam name="T">Null type to return.</typeparam>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<T> Null<T>(Times times = null)
        {
            return Returns(default(T), times);
        }

        /// <summary>Specifies behavior returning the default for a fake.</summary>
        /// <typeparam name="T">Default value type to return.</typeparam>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<T> Default<T>(Times times = null)
        {
            return Returns(default(T), times);
        }

        /// <summary>Specifies a specific exception behavior for a fake.</summary>
        /// <typeparam name="T">Exception type to throw if called.</typeparam>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Throw<T>(Times times = null) where T : Exception, new()
        {
            return Throw(new T(), times);
        }

        /// <summary>Specifies a specific exception behavior for a fake.</summary>
        /// <typeparam name="T">Exception type to throw if called.</typeparam>
        /// <param name="exception">Exception to throw.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Throw<T>(T exception, Times times = null) where T : Exception
        {
            return Set(() => throw exception, times);
        }

        /// <summary>Specifies behavior returning a value for a fake.</summary>
        /// <typeparam name="T">Value type to return.</typeparam>
        /// <param name="value">Value to return.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<T> Returns<T>(T value, Times times = null)
        {
            return Set(() => value, times);
        }

        /// <summary>Specifies behavior returning values for a fake.</summary>
        /// <typeparam name="T">Value type to return.</typeparam>
        /// <param name="value">Value to return first.</param>
        /// <param name="values">Following sequential values to return.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<T> Series<T>(T value, params T[] values)
        {
            int counter = -2;
            return Set(() =>
            {
                counter++;
                if (counter == -1)
                {
                    return value;
                }
                else if (counter < values?.Length)
                {
                    return values[counter];
                }
                else
                {
                    throw new NotImplementedException();
                }
            });
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set(
            Action implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T>(
            Action<T> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2>(
            Action<T1, T2> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3>(
            Action<T1, T2, T3> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4>(
            Action<T1, T2, T3, T4> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5>(
            Action<T1, T2, T3, T4, T5> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6>(
            Action<T1, T2, T3, T4, T5, T6> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7>(
            Action<T1, T2, T3, T4, T5, T6, T7> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<VoidType> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> implementation, Times times = null)
        {
            return new Behavior<VoidType>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<TOut>(
            Func<TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T, TOut>(
            Func<T, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, TOut>(
            Func<T1, T2, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, TOut>(
            Func<T1, T2, T3, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, TOut>(
            Func<T1, T2, T3, T4, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, TOut>(
            Func<T1, T2, T3, T4, T5, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, TOut>(
            Func<T1, T2, T3, T4, T5, T6, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }

        /// <summary>Specifies mock behavior for a fake.</summary>
        /// <param name="implementation">Behavior to invoke for calls.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        public static Behavior<TOut> Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOut>(
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOut> implementation, Times times = null)
        {
            return new Behavior<TOut>(implementation, times);
        }
    }
}
