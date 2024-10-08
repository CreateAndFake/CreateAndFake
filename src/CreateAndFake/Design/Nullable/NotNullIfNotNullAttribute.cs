#if LEGACY // Attribute added in .NET Standard 2.1 & Core 3.0

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
///     Specifies the attached data will not be <c>null</c> when <paramref name="parameterName"/> is not <c>null</c>.
/// </summary>
/// <param name="parameterName"><inheritdoc cref="ParameterName" path="/summary"/></param>
[ExcludeFromCodeCoverage, AttributeUsage(
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue,
    AllowMultiple = true,
    Inherited = false)]
internal sealed class NotNullIfNotNullAttribute(string parameterName) : Attribute
{
    /// <summary>Name of the associated parameter that matches nullability.</summary>
    public string ParameterName { get; } = parameterName;
}

#endif
