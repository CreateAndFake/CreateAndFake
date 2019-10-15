using System;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Creates fake objects.</summary>
    public interface IFaker
    {
        /// <summary>Determines if the type can be faked.</summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if possible; false otherwise.</returns>
        bool Supports<T>();

        /// <summary>Determines if the type can be faked.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if possible; false otherwise.</returns>
        bool Supports(Type type);

        /// <summary>Creates a strict fake where calls fail unless set up.</summary>
        /// <typeparam name="T">Type being faked.</typeparam>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>Handler for fake behavior.</returns>
        Fake<T> Mock<T>(params Type[] interfaces);

        /// <summary>Creates a strict fake where calls fail unless set up.</summary>
        /// <param name="parent">Type being faked.</param>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>Handler for fake behavior.</returns>
        Fake Mock(Type parent, params Type[] interfaces);

        /// <summary>Creates a loose fake with a base default implementation.</summary>
        /// <typeparam name="T">Type being faked.</typeparam>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>Handler for the fake behavior.</returns>
        Fake<T> Stub<T>(params Type[] interfaces);

        /// <summary>Creates a loose fake with a base default implementation.</summary>
        /// <param name="parent">Type being faked.</param>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>Handler for the fake behavior.</returns>
        Fake Stub(Type parent, params Type[] interfaces);

        /// <summary>Creates an instance injected with mocks.</summary>
        /// <typeparam name="T">Instance to be created.</typeparam>
        /// <param name="values">Values to inject instead where possible.</param>
        /// <returns>The created instance with its fakes.</returns>
        Injected<T> InjectMocks<T>(params object[] values);

        /// <summary>Creates an instance injected with stubs.</summary>
        /// <typeparam name="T">Instance to be created.</typeparam>
        /// <param name="values">Values to inject instead where possible.</param>
        /// <returns>The created instance with its fakes.</returns>
        Injected<T> InjectStubs<T>(params object[] values);
    }
}
