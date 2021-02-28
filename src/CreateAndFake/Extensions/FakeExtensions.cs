using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;

#pragma warning disable IDE0060 // Remove unused parameter

namespace CreateAndFake.Fluent
{
    /// <summary>Provides fluent assertions.</summary>
    public static class FakeExtensions
    {
        /// <summary>Ties a method call to fake behavior.</summary>
        /// <param name="fakeCallResult">Result from the fake to setup.</param>
        /// <param name="returnValue">Value to set the call behavior with.</param>
        /// <param name="times">Behavior call limit.</param>
        public static void SetupReturn<T>(this T fakeCallResult, T returnValue, Times times = null)
        {
            SetupReturn(fakeCallResult, Behavior.Returns(returnValue, times));
        }

        /// <summary>Ties a method call to fake behavior.</summary>
        /// <param name="fakeCallResult">Result from the fake to setup.</param>
        /// <param name="behavior">Behavior to set the call behavior with.</param>
        public static void SetupReturn<T>(this T fakeCallResult, Behavior<T> behavior)
        {
            FakeMetaProvider.SetLastCallBehavior(behavior);
        }
    }
}

#pragma warning restore IDE0060 // Remove unused parameter
