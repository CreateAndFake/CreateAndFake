using System;

namespace CreateAndFake
{
    /// <summary>Flag to create the object as a stub.</summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class StubAttribute : Attribute { }
}