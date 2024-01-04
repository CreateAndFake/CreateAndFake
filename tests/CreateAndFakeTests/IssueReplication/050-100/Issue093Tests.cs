using CreateAndFake;
using CreateAndFake.Fluent;
using CreateAndFake.Toolbox.AsserterTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using Xunit;

namespace CreateAndFakeTests.IssueReplication;

/// <summary>Verifies issue is resolved.</summary>
public static class Issue093Tests
{
    public abstract class Provider
    {
        public abstract string Value { get; set; }
    }

    internal sealed class Api
    {
        private readonly Provider _provider;

        internal Api(Provider provider)
        {
            _provider = provider;
        }

        public string Value => _provider.Value;
    }

    [Theory, RandomData]
    internal static void Issue093_AssertFakeCallIntegration([Fake] Provider faked, Api sample, string value)
    {
        faked.Value.SetupReturn(value);

        Tools.Asserter.Throws<FakeVerifyException>(() => "".Assert().Called(faked).And.Is(""));

        sample.Value.Assert().Called(faked).And.Is(value);

        Tools.Asserter.Throws<AssertException>(() => sample.Value.Assert().Called(faked).And.Is(""));
    }
}