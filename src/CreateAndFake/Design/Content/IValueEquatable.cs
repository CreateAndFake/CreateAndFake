namespace CreateAndFake.Design.Content
{
    /// <summary>Provides value equality without changing default reference equality.</summary>
    public interface IValueEquatable
    {
        /// <summary>Compares by value.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <returns>True if equal; false otherwise.</returns>
        bool ValuesEqual(object other);

        /// <summary>Generates a hash based upon value.</summary>
        /// <returns>The generated hash code.</returns>
        int GetValueHash();
    }
}
