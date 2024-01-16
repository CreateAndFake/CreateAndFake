namespace CreateAndFake.Design.Content;

/// <summary>Provides value equality without changing default reference equality.</summary>
public interface IValueEquatable
{
    /// <summary>Compares to <paramref name="other"/> by value.</summary>
    /// <param name="other">Instance to compare with.</param>
    /// <returns><c>true</c> if equal to <paramref name="other"/> by value; <c>false</c> otherwise.</returns>
    bool ValuesEqual(object other);

    /// <summary>Computes an identifying hash code based upon value.</summary>
    /// <returns>The computed hash code.</returns>
    int GetValueHash();
}
