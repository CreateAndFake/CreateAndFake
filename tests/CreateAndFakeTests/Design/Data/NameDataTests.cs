using CreateAndFake.Design.Data;

namespace CreateAndFakeTests.Design.Data;

/// <summary>Verifies behavior.</summary>
public static class NameDataTests
{
    [Fact]
    internal static void Values_Populated()
    {
        NameData.Values.Assert().IsNotEmpty();
    }
}
