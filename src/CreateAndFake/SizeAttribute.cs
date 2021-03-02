using System;

namespace CreateAndFake
{
    /// <summary>Flag to specify size of created collection.</summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class SizeAttribute : Attribute
    {
        /// <summary>Number of items to generate.</summary>
        public int Count { get; }

        /// <summary>Sets up the randomizer capabilities.</summary>
        /// <param name="count">Number of items to generate.</param>
        public SizeAttribute(int count)
        {
            Count = count;
        }
    }
}