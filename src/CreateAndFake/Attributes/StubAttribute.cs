using System;

namespace CreateAndFake
{
    /// <summary>Flag to create the object as a stub.</summary>
    /// <remarks>Stubs are loose fakes with a base default implementation.</remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class StubAttribute : Attribute { }
}