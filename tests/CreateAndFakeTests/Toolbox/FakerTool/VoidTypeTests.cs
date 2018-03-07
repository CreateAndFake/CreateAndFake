using System;
using System.Linq;
using System.Reflection;
using CreateAndFake;
using CreateAndFake.Toolbox.FakerTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreateAndFakeTests.Toolbox.FakerTool
{
    /// <summary>Verifies behavior.</summary>
    [TestClass]
    public sealed class VoidTypeTests
    {
        /// <summary>Verifies the type cannot be normally created.</summary>
        [TestMethod]
        public void VoidType_PrivateConstructor()
        {
            ConstructorInfo constructor = typeof(VoidType).GetConstructors(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Single();
            Tools.Asserter.Is(true, constructor.IsPrivate);
            constructor.Invoke(Array.Empty<object>());
        }
    }
}
