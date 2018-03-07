using System;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFakeTests.TestSamples;
using CreateAndFakeTests.Toolbox.FakerTool.TestSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.FakerTool.Proxy
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class SubclasserTests
    {
        /// <summary>Verifies interfaces can be created.</summary>
        [TestMethod]
        public void Create_InterfacesWork()
        {
            Tools.Asserter.IsNot(null, Subclasser.Create<IFakeSample>());
            Tools.Asserter.IsNot(null, Subclasser.Create<IFakeSample>(typeof(IClashingFakeSample)));
        }

        /// <summary>Verifies classes can be created.</summary>
        [TestMethod]
        public void Create_ClassesWork()
        {
            Tools.Asserter.IsNot(null, Subclasser.Create<AbstractFakeSample>());
            Tools.Asserter.IsNot(null, Subclasser.Create<VirtualFakeSample>());
        }

        /// <summary>Verifies classes can be created.</summary>
        [TestMethod]
        public void Create_BothWork()
        {
            Tools.Asserter.IsNot(null, Subclasser.Create<AbstractFakeSample>(typeof(IFakeSample)));
            Tools.Asserter.IsNot(null, Subclasser.Create<VirtualFakeSample>(
                typeof(IFakeSample), typeof(IClashingFakeSample)));
        }

        /// <summary>Verifies IFaked is included by default default.</summary>
        [TestMethod]
        public void Create_IFakedDefault()
        {
            Tools.Asserter.Is(true, Subclasser.Create<object>() is IFaked);
            Tools.Asserter.IsNot(null, Subclasser.Create(null, null));
        }

        /// <summary>Verifies meta is implemented.</summary>
        [TestMethod]
        public void Create_IFakedFunctional()
        {
            Tools.Asserter.IsNot(null, Subclasser.Create<IFaked>().FakeMeta);
        }

        /// <summary>Verifies you can't subclass two concrete types.</summary>
        [TestMethod]
        public void Create_OnlyInterfaces()
        {
            Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create<DataSample>(typeof(object)));
        }

        /// <summary>Verifies sealed typs can't be subclassed.</summary>
        [TestMethod]
        public void Create_SealedTypesThrow()
        {
            Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create<string>());
        }

        /// <summary>Verifies types are reused.</summary>
        [TestMethod]
        public void CreateInfo_NoDuplicatesCreated()
        {
            Tools.Asserter.Is(Subclasser.CreateInfo(typeof(IFakeSample), typeof(IClashingFakeSample)),
                Subclasser.CreateInfo(typeof(IClashingFakeSample), typeof(IFakeSample)));
        }

        /// <summary>Verifies duplicate interfaces are removed.</summary>
        [TestMethod]
        public void CreateInfo_IgnoreDupeInterfaces()
        {
            Tools.Asserter.Is(Subclasser.CreateInfo(typeof(IFakeSample)),
                Subclasser.CreateInfo(typeof(IFakeSample), typeof(IFakeSample)));
        }

        /// <summary>Verifies types with defined generics works.</summary>
        [TestMethod]
        public void Create_DefinedGenericsWork()
        {
            Tools.Asserter.IsNot(null, Subclasser.Create<ConstraintSample<int, DataSample>>());
            Tools.Asserter.IsNot(null, Subclasser.Create<ConstraintSample<bool, DataSample>>());
        }

        /// <summary>Verifies types with undefined generics throw.</summary>
        [TestMethod]
        public void Create_UndefinedGenericsThrow()
        {
            Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create(typeof(ConstraintSample<,>)));
        }

        /// <summary>Verifies pointer types throw.</summary>
        [TestMethod]
        public void Create_PointersThrow()
        {
            Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create(typeof(void*)));
        }

        /// <summary>Verifies internal types throw.</summary>
        [TestMethod]
        public void Create_InternalTypesThrow()
        {
            Tools.Asserter.Throws<ArgumentException>(() => Subclasser.Create<InternalSample>());
        }
    }
}
