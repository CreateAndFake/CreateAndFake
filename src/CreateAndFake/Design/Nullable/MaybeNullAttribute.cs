#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER) // Attribute added in .NET Standard 2.1.

namespace System.Diagnostics.CodeAnalysis;

/// <summary>Specifies the attached data may actually be <c>null</c>.</summary>
[ExcludeFromCodeCoverage, AttributeUsage(
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.ReturnValue,
    Inherited = false)]
internal sealed class MaybeNullAttribute : Attribute { }

#endif
