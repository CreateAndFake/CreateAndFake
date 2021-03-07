using System;
using System.Diagnostics.CodeAnalysis;
using CreateAndFake.Design.Content;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Mock behavior for fakes.</summary>
    /// <typeparam name="T">Return type.</typeparam>
    public sealed class Behavior<T> : Behavior
    {
        /// <summary>Initializes a new instance of the <see cref="Behavior{T}"/> class.</summary>
        /// <param name="implementation">Set behavior to run.</param>
        /// <param name="times">Behavior call limit.</param>
        public Behavior(Delegate implementation, Times times = null) : base(implementation, times, 0) { }

        /// <summary>Initializes a new instance of the <see cref="Behavior{T}"/> class.</summary>
        /// <param name="implementation">Set behavior to run.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <param name="calls">Starting number of calls.</param>
        private Behavior(Delegate implementation, Times times, int calls) : base(implementation, times, calls) { }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <returns>Clone that is equal in value to the current instance.</returns>
        public override IDeepCloneable DeepClone()
        {
            return new Behavior<T>(Implementation, Limit, Calls);
        }

        /// <summary>Specifies exception behavior for a fake.</summary>
        /// <param name="times">Behavior call limit.</param>
        [SuppressMessage("Microsoft.Design",
            "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "Distinct behavior per generic intended.")]
        public static new Behavior<T> Error(Times times = null)
        {
            return Throw<NotImplementedException>(times);
        }

        /// <summary>Specifies a specific exception behavior for a fake.</summary>
        /// <typeparam name="TException">Exception type to throw if called.</typeparam>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "Distinct behavior per generic intended.")]
        public static new Behavior<T> Throw<TException>(Times times = null) where TException : Exception, new()
        {
            return Throw(new TException(), times);
        }

        /// <summary>Specifies a specific exception behavior for a fake.</summary>
        /// <typeparam name="TException">Exception type to throw if called.</typeparam>
        /// <param name="exception">Exception to throw.</param>
        /// <param name="times">Behavior call limit.</param>
        /// <returns>Instance to set up the mock with.</returns>
        [SuppressMessage("Microsoft.Design",
            "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "Distinct behavior per generic intended.")]
        public static new Behavior<T> Throw<TException>(TException exception, Times times = null) where TException : Exception
        {
            return Set<T>(() => throw exception, times);
        }
    }
}
