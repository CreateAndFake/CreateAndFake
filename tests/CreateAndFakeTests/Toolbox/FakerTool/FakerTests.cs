using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class FakerTests
    {
        /// <summary>Verifies valuer can be null.</summary>
        [TestMethod]
        public void New_NullValuerValid()
        {
            Tools.Asserter.IsNot(null, new Faker(null));
        }

        /// <summary>Verifies the sample can be created.</summary>
        [TestMethod]
        public void Mock_SampleWorks()
        {
            Fake<DataHolderSample> sample = new Faker(Tools.Valuer).Mock<DataHolderSample>();
            Tools.Asserter.Is(true, sample.ThrowByDefault);
            Tools.Asserter.Throws<FakeCallException>(() => sample.Dummy.HasNested(sample.Dummy.NestedValue));
        }

        /// <summary>Verifies the sample can be created.</summary>
        [TestMethod]
        public void Stub_SampleWorks()
        {
            Fake<DataHolderSample> sample = new Faker(Tools.Valuer).Stub<DataHolderSample>();
            Tools.Asserter.Is(false, sample.ThrowByDefault);
            Tools.Asserter.Is(false, sample.Dummy.HasNested(null));
        }

        /// <summary>Verifies invalid types aren't supported.</summary>
        [TestMethod]
        public void Supports_InvalidTypesFalse()
        {
            Tools.Asserter.Is(false, Tools.Faker.Supports<int>());
            Tools.Asserter.Is(false, Tools.Faker.Supports<Array>());
            Tools.Asserter.Is(false, Tools.Faker.Supports<InternalSample>());
            Tools.Asserter.Is(false, Tools.Faker.Supports(typeof(void*)));
            Tools.Asserter.Is(false, Tools.Faker.Supports(typeof(ConstraintSample<,>)));
        }
    }
}
