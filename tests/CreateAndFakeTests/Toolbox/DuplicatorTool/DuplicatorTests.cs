using System;
using CreateAndFake;
using CreateAndFake.Toolbox.DuplicatorTool;
using CreateAndFake.Toolbox.FakerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.DuplicatorTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class DuplicatorTests
    {
        /// <summary>Verifies nulls are valid.</summary>
        [TestMethod]
        public void New_NullHintsValid()
        {
            Tools.Asserter.IsNot(null, new Duplicator(Tools.Asserter, true, null));
            Tools.Asserter.IsNot(null, new Duplicator(Tools.Asserter, false, null));
        }

        /// <summary>Verifies an exception throws when no hint matches.</summary>
        [TestMethod]
        public void Copy_MissingMatchThrows()
        {
            Tools.Asserter.Throws<NotSupportedException>(
                () => new Duplicator(Tools.Asserter, false).Copy(new object()));
        }

        /// <summary>Verifies the duplicator can copy null.</summary>
        [TestMethod]
        public void Copy_NullWorks()
        {
            Tools.Asserter.Is(null, new Duplicator(Tools.Asserter, false).Copy<object>(null));
        }

        /// <summary>Verifies hint behavior works.</summary>
        [TestMethod]
        public void Copy_ValidHintWorks()
        {
            object data = new object();

            Fake<CopyHint> hint = Tools.Faker.Mock<CopyHint>();
            hint.Setup(
                m => m.TryCopy(data, Arg.Any<DuplicatorChainer>()),
                Behavior.Returns((true, data), Times.Once));

            Tools.Asserter.Is(data, new Duplicator(Tools.Asserter, false, hint.Dummy).Copy(data));
            hint.Verify(Times.Once);
        }
    }
}
