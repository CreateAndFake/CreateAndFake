using System;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using Xunit;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool
{
    /// <summary>Verifies behavior.</summary>
    public static class DuplicatorTests
    {
        /// <summary>Verifies null reference exceptions are prevented.</summary>
        [Fact]
        public static void Duplicator_GuardsNulls()
        {
            Tools.Tester.PreventsNullRefException<Duplicator>();
        }

        /// <summary>Verifies nulls are valid.</summary>
        [Fact]
        public static void New_NullHintsValid()
        {
            Tools.Asserter.IsNot(null, new Duplicator(Tools.Asserter, true, null));
            Tools.Asserter.IsNot(null, new Duplicator(Tools.Asserter, false, null));
        }

        /// <summary>Verifies an exception throws when no hint matches.</summary>
        [Fact]
        public static void Copy_MissingMatchThrows()
        {
            Tools.Asserter.Throws<NotSupportedException>(
                () => new Duplicator(Tools.Asserter, false).Copy(new object()));
        }

        /// <summary>Verifies the duplicator can copy null.</summary>
        [Fact]
        public static void Copy_NullWorks()
        {
            Tools.Asserter.Is(null, new Duplicator(Tools.Asserter, false).Copy<object>(null));
        }

        /// <summary>Verifies hint behavior works.</summary>
        [Theory, RandomData]
        public static void Copy_ValidHintWorks(object data, Fake<CopyHint> hint)
        {
            hint.Setup(
                m => m.TryCopy(data, Arg.Any<DuplicatorChainer>()),
                Behavior.Returns((true, data), Times.Once));

            Tools.Asserter.Is(data, new Duplicator(Tools.Asserter, false, hint.Dummy).Copy(data));
            hint.Verify(Times.Once);
        }
    }
}
