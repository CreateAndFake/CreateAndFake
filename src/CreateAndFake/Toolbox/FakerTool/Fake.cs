using System;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Manages faking objects.</summary>
    public class Fake
    {
        /// <summary>Faked implementation.</summary>
        public IFaked Dummy { get; }

        /// <summary>How to compare call data.</summary>
        protected IValuer Valuer { get; }

        /// <summary>Determines behavior when missing set behavior for a call.</summary>
        public bool ThrowByDefault
        {
            get => Dummy.FakeMeta.ThrowByDefault;
            set => Dummy.FakeMeta.ThrowByDefault = value;
        }

        /// <summary>Sets up the fake harness.</summary>
        /// <param name="fake">Faked implementation.</param>
        /// <param name="valuer">How to compare call data.</param>
        public Fake(IFaked fake, IValuer valuer = null)
        {
            Dummy = fake;
            Valuer = valuer;
        }

        /// <summary>For converting fakes to different types.</summary>
        /// <param name="baseFake">Created fake with the extra type.</param>
        protected Fake(Fake baseFake) : this(baseFake.Dummy, baseFake.Valuer) { }

        /// <summary>Ties a method call to fake behavior.</summary>
        /// <param name="methodName">Method name of the call.</param>
        /// <param name="args">Args to match on the call.</param>
        /// <param name="callback">Fake behavior to invoke.</param>
        /// <returns>Representation of the call.</returns>
        public void Setup(string methodName, object[] args, Behavior callback)
        {
            Setup(methodName, Array.Empty<Type>(), args, callback);
        }

        /// <summary>Ties a method call to fake behavior.</summary>
        /// <param name="methodName">Method name of the call.</param>
        /// <param name="generics">Generics tied to the call.</param>
        /// <param name="args">Args to match on the call.</param>
        /// <param name="callback">Fake behavior to invoke.</param>
        /// <returns>Representation of the call.</returns>
        public void Setup(string methodName, Type[] generics, object[] args, Behavior callback)
        {
            Dummy.FakeMeta.SetCallBehavior(new CallData(methodName, generics, args, Valuer), callback);
        }

        /// <summary>Verifies all behaviors with associated times were called as expected.</summary>
        /// <param name="total">Expected total number of calls to test as well.</param>
        public void VerifyAll(Times total = null)
        {
            Dummy.FakeMeta.Verify();
            if (total != null)
            {
                VerifyTotalCalls(total);
            }
        }

        /// <summary>Verifies the number of calls made.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="methodName">Method name of the call.</param>
        /// <param name="args">Args to match on the call.</param>
        public void Verify(Times times, string methodName, object[] args)
        {
            Verify(times, methodName, Array.Empty<Type>(), args);
        }

        /// <summary>Verifies the number of calls made.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="methodName">Method name of the call.</param>
        /// <param name="generics">Generics tied to the call.</param>
        /// <param name="args">Args to match on the call.</param>
        public void Verify(Times times, string methodName, Type[] generics, object[] args)
        {
            Verify(times, new CallData(methodName, generics, args, Valuer));
        }

        /// <summary>Verifies the number of calls made.</summary>
        /// <param name="times">Expected number of calls.</param>
        /// <param name="call">Call to verify.</param>
        private void Verify(Times times, CallData call)
        {
            Dummy.FakeMeta.Verify(times, call);
        }

        /// <summary>Verifies the total number of calls made.</summary>
        /// <param name="times">Expected total.</param>
        public void VerifyTotalCalls(Times times)
        {
            Dummy.FakeMeta.VerifyTotalCalls(times);
        }
    }
}
