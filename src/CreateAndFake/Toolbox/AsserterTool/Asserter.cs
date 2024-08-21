using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.AsserterTool.Fluent;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool;

/// <summary>Handles common test scenarios.</summary>
/// <param name="gen"><inheritdoc cref="Gen" path="/summary"/></param>
/// <param name="valuer"><inheritdoc cref="Valuer" path="/summary"/></param>
/// <exception cref="ArgumentNullException">If given a <c>null</c> parameter.</exception>
public class Asserter(IRandom gen, IValuer valuer)
{
    /// <summary>Core randomizer with a potential seed for logging.</summary>
    protected IRandom Gen { get; } = gen ?? throw new ArgumentNullException(nameof(gen));

    /// <summary>Handles comparisons for assertion checks.</summary>
    protected IValuer Valuer { get; } = valuer ?? throw new ArgumentNullException(nameof(valuer));

    /// <summary>Runs each case and aggregates exceptions.</summary>
    /// <param name="cases">Assert cases.</param>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Rethrows all at end.")]
    public virtual void CheckAll(params Action[] cases)
    {
        if (cases == null)
        {
            return;
        }

        Exception?[] errors = new Exception[cases.Length];
        for (int i = 0; i < errors.Length; i++)
        {
            try
            {
                cases[i].Invoke();
                errors[i] = null;
            }
            catch (Exception e)
            {
                errors[i] = e;
            }
        }

        if (errors.Any(e => e != null))
        {
            throw new AggregateException("Cases failed: " +
                string.Join(", ", Enumerable.Range(0, errors.Length).Where(i => errors[i] != null)) + " -",
                errors.Where(e => e != null).Select(e => e!));
        }
    }

    /// <inheritdoc cref="Fail(Exception,string)"/>
    public virtual void Fail(string? details = null)
    {
        throw new AssertException("Test failed.", details, Gen.InitialSeed);
    }

    /// <summary>Throws an assert exception.</summary>
    /// <param name="exception">Exception that occurred.</param>
    /// <param name="details">Optional failure details to include.</param>
    public virtual void Fail(Exception exception, string? details = null)
    {
        throw new AssertException("Test failed.", details, Gen.InitialSeed, exception);
    }

    /// <param name="actual"><inheritdoc cref="AssertObjectBase{T}.Actual" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertObjectBase{T}.Is"/>
    public void Is(object? expected, object? actual, string? details = null)
    {
        _ = new AssertObject(Gen, Valuer, actual).Is(expected, details);
    }

    /// <param name="actual"><inheritdoc cref="AssertObjectBase{T}.Actual" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertObjectBase{T}.IsNot"/>
    public void IsNot(object? expected, object? actual, string? details = null)
    {
        _ = new AssertObject(Gen, Valuer, actual).IsNot(expected, details);
    }

    /// <param name="collection"><inheritdoc cref="AssertGroupBase{T}.Collection" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertGroupBase{T}.IsEmpty"/>
    public virtual void IsEmpty(IEnumerable? collection, string? details = null)
    {
        _ = new AssertGroup(Gen, Valuer, collection).IsEmpty(details);
    }

    /// <param name="collection"><inheritdoc cref="AssertGroupBase{T}.Collection" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertGroupBase{T}.IsNotEmpty"/>
    public virtual void IsNotEmpty(IEnumerable? collection, string? details = null)
    {
        _ = new AssertGroup(Gen, Valuer, collection).IsNotEmpty(details);
    }

    /// <param name="collection"><inheritdoc cref="AssertGroupBase{T}.Collection" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertGroupBase{T}.HasCount"/>
    public virtual void HasCount(int count, IEnumerable? collection, string? details = null)
    {
        _ = new AssertGroup(Gen, Valuer, collection).HasCount(count, details);
    }

    /// <param name="actual"><inheritdoc cref="AssertObjectBase{T}.Actual" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertObjectBase{T}.ReferenceEqual"/>
    public virtual void ReferenceEqual(object? expected, object? actual, string? details = null)
    {
        _ = new AssertObject(Gen, Valuer, actual).ReferenceEqual(expected, details);
    }

    /// <param name="actual"><inheritdoc cref="AssertObjectBase{T}.Actual" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertObjectBase{T}.ReferenceNotEqual"/>
    public virtual void ReferenceNotEqual(object? expected, object? actual, string? details = null)
    {
        _ = new AssertObject(Gen, Valuer, actual).ReferenceNotEqual(expected, details);
    }

    /// <param name="actual"><inheritdoc cref="AssertObjectBase{T}.Actual" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertObjectBase{T}.ValuesEqual(object?, string?)"/>
    public virtual void ValuesEqual(object? expected, object? actual, string? details = null)
    {
        _ = new AssertObject(Gen, Valuer, actual).ValuesEqual(expected, details);
    }

    /// <param name="actual"><inheritdoc cref="AssertObjectBase{T}.Actual" path="/summary"/></param>
    /// <returns></returns>
    /// <inheritdoc cref="AssertObjectBase{T}.ValuesNotEqual(object?, string?)"/>
    public virtual void ValuesNotEqual(object? expected, object? actual, string? details = null)
    {
        _ = new AssertObject(Gen, Valuer, actual).ValuesNotEqual(expected, details);
    }

    /// <param name="behavior"><inheritdoc cref="AssertBehaviorBase{T}.Throws" path="/summary"/></param>
    /// <inheritdoc cref="AssertBehaviorBase{T}.Throws"/>
    public virtual T Throws<T>(Action? behavior, string? details = null) where T : Exception
    {
        return new AssertBehavior(Gen, Valuer, behavior).Throws<T>(details);
    }

    /// <param name="behavior"><inheritdoc cref="AssertBehaviorBase{T}.Throws" path="/summary"/></param>
    /// <inheritdoc cref="AssertBehaviorBase{T}.Throws"/>
    public virtual T Throws<T>(Func<object?>? behavior, string? details = null) where T : Exception
    {
        return new AssertBehavior(Gen, Valuer, behavior).Throws<T>(details);
    }
}
