using System;
using CreateAndFake.Toolbox.FakerTool.Proxy;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Creates fake objects.</summary>
    public sealed class Faker : IFaker
    {
        /// <summary>Handles comparisons.</summary>
        private readonly IValuer _valuer;

        /// <summary>Sets up the asserter capabilities.</summary>
        /// <param name="valuer">Handles comparisons.</param>
        public Faker(IValuer valuer)
        {
            _valuer = valuer;
        }

        /// <summary>Determines if the type can be faked.</summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if possible; false otherwise.</returns>
        public bool Supports<T>()
        {
            return Subclasser.Supports<T>();
        }

        /// <summary>Determines if the type can be faked.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if possible; false otherwise.</returns>
        public bool Supports(Type type)
        {
            return Subclasser.Supports(type);
        }

        /// <summary>Creates a strict fake where calls fail unless set up.</summary>
        /// <typeparam name="T">Type being faked.</typeparam>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>Handler for fake behavior.</returns>
        public Fake<T> Mock<T>(params Type[] interfaces)
        {
            return new Fake<T>(Subclasser.Create(typeof(T), interfaces), _valuer);
        }

        /// <summary>Creates a strict fake where calls fail unless set up.</summary>
        /// <param name="parent">Type being faked.</param>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>Handler for fake behavior.</returns>
        public Fake Mock(Type parent, params Type[] interfaces)
        {
            return new Fake(Subclasser.Create(parent, interfaces), _valuer);
        }

        /// <summary>Creates a loose fake with a base default implementation.</summary>
        /// <typeparam name="T">Type being faked.</typeparam>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>Handler for the fake behavior.</returns>
        public Fake<T> Stub<T>(params Type[] interfaces)
        {
            Fake<T> fake = Mock<T>(interfaces);
            fake.ThrowByDefault = false;
            return fake;
        }

        /// <summary>Creates a loose fake with a base default implementation.</summary>
        /// <param name="parent">Type being faked.</param>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>Handler for the fake behavior.</returns>
        public Fake Stub(Type parent, params Type[] interfaces)
        {
            Fake fake = Mock(parent, interfaces);
            fake.ThrowByDefault = false;
            return fake;
        }
    }
}
