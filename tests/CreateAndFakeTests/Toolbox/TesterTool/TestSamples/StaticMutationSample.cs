using CreateAndFakeTests.TestSamples;

namespace CreateAndFakeTests.Toolbox.TesterTool.TestSamples;

/// <summary>For testing.</summary>
internal static class StaticMutationSample
{
    /// <summary>For testing.</summary>
    internal static void Mutate(DataHolderSample data)
    {
        data.NestedValue = null;
    }
}
