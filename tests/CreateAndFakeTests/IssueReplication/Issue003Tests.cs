using System.Diagnostics.CodeAnalysis;
using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue003Tests
    {
        /// <summary>For testing.</summary>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesVisible", Justification = "For testing.")]
        public abstract class RefHolder
        {
            /// <summary>For testing.</summary>
            public abstract int TestMethodA(out int value);

            /// <summary>For testing.</summary>
            public abstract int TestMethodB(ref int value);
        }

        [Theory, RandomData]
        internal static void Issue_FakeHandlesOut(Fake<RefHolder> sample, int testValue, int result)
        {
            sample.Setup(
                d => d.TestMethodA(out Arg.AnyRef<int>().Var),
                Behavior.Set((OutRef<int> v) =>
                {
                    v.Var = testValue;
                    return result;
                }));

            sample.Dummy.TestMethodA(out int newValue).Assert().Is(result);
            sample.VerifyAll();
            newValue.Assert().Is(testValue);
        }

        [Theory, RandomData]
        internal static void Issue_FakeHandlesRef(Fake<RefHolder> sample, int result)
        {
            int testValue = 0;
            sample.Setup(
                d => d.TestMethodB(ref Arg.AnyRef<int>().Var),
                Behavior.Set((OutRef<int> v) =>
                {
                    v.Var -= 5;
                    return result;
                }));

            sample.Dummy.TestMethodB(ref testValue).Assert().Is(result);
            sample.VerifyAll();
            testValue.Assert().Is(-5);
        }
    }
}
