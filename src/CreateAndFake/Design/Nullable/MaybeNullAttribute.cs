#if LEGACY // Attribute added in .NET Standard 2.1 & Core 3.0

namespace System.Diagnostics.CodeAnalysis;

/// <summary>Specifies the attached data may actually be <c>null</c>.</summary>
[ExcludeFromCodeCoverage, AttributeUsage(
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.ReturnValue,
    Inherited = false)]
internal sealed class MaybeNullAttribute : Attribute { }

#endif
