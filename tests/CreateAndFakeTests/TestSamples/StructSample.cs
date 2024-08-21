using System.Diagnostics.CodeAnalysis;
using CreateAndFake.Design.Content;

namespace CreateAndFakeTests.TestSamples;

#pragma warning disable IDE0250 // Struct can be made 'readonly'
#pragma warning disable IDE0251 // Member can be made 'readonly'

/// <summary>For testing.</summary>
[ExcludeFromCodeCoverage]
internal struct StructSample(string stringValue) : IEquatable<StructSample>, IComparable<StructSample>, IComparable
{
    /// <summary>For testing.</summary>
    public string StringValue { get; } = stringValue;

    /// <summary>For testing.</summary>
    public int CompareTo(StructSample other)
    {
        return ValueComparer.Use.Compare(this, other);
    }

    /// <summary>For testing.</summary>
    public int CompareTo(object obj)
    {
        return ValueComparer.Use.Compare(this, obj);
    }

    /// <summary>For testing.</summary>
    public override bool Equals(object obj)
    {
        return (obj is StructSample sample) && Equals(sample);
    }

    /// <summary>For testing.</summary>
    public bool Equals(StructSample other)
    {
        return ValueComparer.Use.Equals(StringValue, other.StringValue);
    }

    /// <summary>For testing.</summary>
    public override int GetHashCode()
    {
        return ValueComparer.Use.GetHashCode(StringValue);
    }

    /// <summary>For testing.</summary>
    public static bool operator ==(StructSample left, StructSample right)
    {
        return left.Equals(right);
    }

    /// <summary>For testing.</summary>
    public static bool operator !=(StructSample left, StructSample right)
    {
        return !(left == right);
    }

    /// <summary>For testing.</summary>
    public static bool operator <(StructSample left, StructSample right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>For testing.</summary>
    public static bool operator <=(StructSample left, StructSample right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>For testing.</summary>
    public static bool operator >(StructSample left, StructSample right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>For testing.</summary>
    public static bool operator >=(StructSample left, StructSample right)
    {
        return left.CompareTo(right) >= 0;
    }
}

#pragma warning restore IDE0250 // Struct can be made 'readonly'
#pragma warning restore IDE0251 // Member can be made 'readonly'
