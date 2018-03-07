using System;

namespace CreateAndFake.Toolbox.DuplicatorTool
{
    /// <summary>Deep clones objects.</summary>
    public interface IDuplicator
    {
        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <returns>The duplicate.</returns>
        /// <exception cref="NotSupportedException">If no hint supports cloning the object.</exception>
        T Copy<T>(T source);
    }
}
