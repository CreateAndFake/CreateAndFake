﻿using System.Diagnostics.CodeAnalysis;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>For matching out and ref arguments.</summary>
    /// <typeparam name="T">Argument type to match.</typeparam>
    public sealed class OutRef<T>
    {
        /// <summary>Used as the out/ref argument.</summary>
        [SuppressMessage("Microsoft.Design",
            "CA1051:DoNotDeclareVisibleInstanceFields",
            Justification = "Required to match out/ref."),
        SuppressMessage("Sonar",
            "S1104:AvoidPublicFields",
            Justification = "Required to match out/ref.")]
        public T Var = default;
    }
}