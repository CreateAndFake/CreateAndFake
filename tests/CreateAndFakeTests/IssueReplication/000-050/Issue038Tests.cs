using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.IssueReplication
{
    /// <summary>Verifies issue is resolved.</summary>
    public static class Issue038Tests
    {
        public interface ISample
        {
            string Value { get; set; }

            int Check(InnerSample value);

            int Check(string item1, string item2);
        }

        public class InnerSample
        {
            public string Value { get; set; }
        }

        [Theory, RandomData]
        internal static void Issue038_FluentStubSetup([Stub] ISample sample, string value)
        {
            sample.Value.Assert().Is(null);
            sample.Value.SetupReturn(value);
            sample.Value.Assert().Is(value);
        }

        [Theory, RandomData]
        internal static void Issue038_FluentFakeSetup([Fake] ISample sample, string value)
        {
            sample.Value.Assert().IsNot(null);
            sample.Value.SetupReturn(value);
            sample.Value.Assert().Is(value);
        }

        [Theory, RandomData]
        internal static void Issue038_FluentArgsByValue([Stub] ISample sample, InnerSample item, int result)
        {
            sample.Check(item).SetupReturn(result);
            sample.Check(Tools.Duplicator.Copy(item)).Assert().Is(result);
        }

        [Theory, RandomData]
        internal static void Issue038_FluentArgWorks([Stub] ISample sample, string item1, string item2, int result)
        {
            sample.Check(Arg.Any<string>(), Arg.Where<string>(s => s == item2)).SetupReturn(result);
            sample.Check(item2, item1).Assert().Is(0);
            sample.Check(item1, item2).Assert().Is(result);
        }
    }
}