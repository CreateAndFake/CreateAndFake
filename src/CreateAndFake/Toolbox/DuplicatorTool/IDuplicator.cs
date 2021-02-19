﻿using System;

namespace CreateAndFake.Toolbox.DuplicatorTool
{
    /// <summary>Deep clones objects.</summary>
    public interface IDuplicator
    {
        /// <summary>Deep clones an object.</summary>
        /// <typeparam name="T">Type being cloned.</typeparam>
        /// <param name="source">Object to clone.</param>
        /// <returns>The duplicate.</returns>
        /// <exception cref="NotSupportedException">If no hint supports cloning the object.</exception>
        /// <exception cref="InsufficientExecutionStackException">If infinite recursion occurs.</exception>
        T Copy<T>(T source);

        /// <summary>Adds the hint to be used for cloning.</summary>
        /// <param name="hint">Hint to add.</param>
        /// <remarks>Should only be modified in module inintializers.</remarks>
        void AddHint(CopyHint hint);
    }
}
