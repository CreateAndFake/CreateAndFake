using System;
using System.Linq;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent;

/// <summary>Handles assertion calls.</summary>
/// <param name="gen">Core value random handler.</param>
/// <param name="valuer">Handles comparisons.</param>
/// <param name="actual">Object to compare with.</param>
public abstract class AssertObjectBase<T>(IRandom gen, IValuer valuer, object actual) where T : AssertObjectBase<T>
{
    /// <summary>Core value random handler.</summary>
    protected IRandom Gen { get; } = gen ?? throw new ArgumentNullException(nameof(gen));

    /// <summary>Handles comparisons.</summary>
    protected IValuer Valuer { get; } = valuer ?? throw new ArgumentNullException(nameof(valuer));

    /// <summary>Object to compare with.</summary>
    protected object Actual { get; } = actual;

    /// <summary>Verifies two objects are equal by value.</summary>
    /// <param name="expected">Object to compare against.</param>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
    public AssertChainer<T> Is(object expected, string details = null)
    {
        return ValuesEqual(expected, details);
    }

    /// <summary>Verifies two objects are unequal by value.</summary>
    /// <param name="expected">Object to compare against.</param>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
    public AssertChainer<T> IsNot(object expected, string details = null)
    {
        return ValuesNotEqual(expected, details);
    }

    /// <summary>Verifies two objects are equal by reference.</summary>
    /// <param name="expected">Object to compare against.</param>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
    public virtual AssertChainer<T> ReferenceEqual(object expected, string details = null)
    {
        if (!ReferenceEquals(expected, Actual))
        {
            throw new AssertException("References failed to equal.", details, Gen.InitialSeed);
        }
        return ToChainer();
    }

    /// <summary>Verifies two objects are not equal by reference.</summary>
    /// <param name="expected">Object to compare against.</param>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
    public virtual AssertChainer<T> ReferenceNotEqual(object expected, string details = null)
    {
        if (ReferenceEquals(expected, Actual))
        {
            throw new AssertException("References failed to not equal.", details, Gen.InitialSeed);
        }
        return ToChainer();
    }

    /// <summary>Verifies two objects are equal by value.</summary>
    /// <param name="expected">Object to compare against.</param>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
    public virtual AssertChainer<T> ValuesEqual(object expected, string details = null)
    {
        Difference[] differences = Valuer.Compare(expected, Actual).ToArray();
        if (differences.Length > 0)
        {
            throw new AssertException($"Value equality failed for type '{GetTypeName(expected)}'.",
                details, Gen.InitialSeed, string.Join<Difference>(Environment.NewLine, differences));
        }
        return ToChainer();
    }

    /// <summary>Verifies two objects are unequal by value.</summary>
    /// <param name="expected">Object to compare against.</param>
    /// <param name="details">Optional failure details.</param>
    /// <returns>Chainer to make additional assertions with.</returns>
    /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
    public virtual AssertChainer<T> ValuesNotEqual(object expected, string details = null)
    {
        if (!Valuer.Compare(expected, Actual).Any())
        {
            throw new AssertException(
                $"Value inequality failed for type '{GetTypeName(expected)}'.",
                details, Gen.InitialSeed, expected?.ToString());
        }
        return ToChainer();
    }

    /// <summary>Throws an assert exception.</summary>
    /// <param name="details">Optional failure details.</param>
    public virtual void Fail(string details = null)
    {
        throw new AssertException("Test failed.", details, Gen.InitialSeed, Actual?.ToString());
    }

    /// <summary>Find a type name to use for messages.</summary>
    /// <param name="expected">Object to compare against.</param>
    /// <returns>Found name to use.</returns>
    protected string GetTypeName(object expected)
    {
        return ExpandTypeName((expected ?? Actual)?.GetType());
    }

    /// <summary>Builds type name with generic argument names.</summary>
    /// <param name="type">Type to describe.</param>
    /// <returns>Built name.</returns>
    private static string ExpandTypeName(Type type)
    {
        if (type != null && type.IsGenericType)
        {
            return string.Concat(
                type.Name.AsSpan(0, type.Name.IndexOf('`', StringComparison.InvariantCulture)),
                "<",
                string.Join(",", type.GetGenericArguments().Select(ExpandTypeName)),
                ">");
        }
        else
        {
            return type?.Name;
        }
    }

    /// <summary>Converts the instance to a chainer.</summary>
    /// <returns>The created chainer.</returns>
    protected internal AssertChainer<T> ToChainer()
    {
        return new AssertChainer<T>((T)this, Gen, Valuer);
    }
}
