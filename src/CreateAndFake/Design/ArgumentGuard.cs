using System;

namespace CreateAndFake.Design;

/// <summary>Common argument exception cases.</summary>
internal static class ArgumentGuard
{
    /// <summary>Prevents further execution if the parameter is null.</summary>
    /// <param name="value">Parameter value.</param>
    /// <param name="name">Name of the parameter.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> is null.</exception>
    internal static void ThrowIfNull(object value, string name)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }
    }
}
