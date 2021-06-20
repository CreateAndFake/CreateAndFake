using System;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls for delegates.</summary>
    public abstract class AssertBehaviorBase<T> : AssertObjectBase<T> where T : AssertBehaviorBase<T>
    {
        /// <summary>Delegate to check.</summary>
        protected Delegate Behavior { get; }

        /// <summary>Initializes a new instance of the <see cref="AssertBehaviorBase{T}"/> class.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="behavior">Delegate to check.</param>
        protected AssertBehaviorBase(IRandom gen, IValuer valuer, Delegate behavior) : base(gen, valuer, behavior)
        {
            Behavior = behavior;
        }

        /// <summary>Verifies the given behavior throws an exception.</summary>
        /// <typeparam name="TException">Exception type expected.</typeparam>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual TException Throws<TException>(string details = null) where TException : Exception
        {
            string errorMessage = $"Expected exception of type '{typeof(TException).FullName}'.";
            try
            {
                if (Behavior is Action action)
                {
                    action.Invoke();
                }
                else
                {
                    object result = ((dynamic)Behavior)?.Invoke();
                    (result as IDisposable)?.Dispose();
                    (result as IAsyncDisposable)?.DisposeAsync().AsTask().Wait();
                }
            }
            catch (TException e)
            {
                return e;
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions.Count == 1 && e.InnerExceptions[0] is TException actual)
                {
                    return actual;
                }
                else
                {
                    throw new AssertException(errorMessage, details, Gen.InitialSeed, e);
                }
            }
            catch (Exception e)
            {
                throw new AssertException(errorMessage, details, Gen.InitialSeed, e);
            }

            throw new AssertException(errorMessage, details, Gen.InitialSeed);
        }
    }
}
