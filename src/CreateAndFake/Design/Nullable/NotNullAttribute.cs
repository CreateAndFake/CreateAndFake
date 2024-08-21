#if LEGACY // Attribute added in .NET Standard 2.1 & Core 3.0

namespace System.Diagnostics.CodeAnalysis;

/// <summary>Specifies the attached data will not be <c>null</c> when the call returns.</summary>
[ExcludeFromCodeCoverage, AttributeUsage(
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.ReturnValue,
    Inherited = false)]
internal sealed class NotNullAttribute : Attribute { }

#endif