using System;

namespace CreateAndFake
{
    /// <summary>Flag to create the object as a random stub.</summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class FakeAttribute : Attribute { }
}