namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Provides value equality with the aid of an <see cref="IValuer"/>.</summary>
    public interface IValuerEquatable
    {
        /// <summary>Compares to <paramref name="other"/> by value.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>True if equal to <paramref name="other"/> by value; false otherwise.</returns>
        bool ValuesEqual(object other, IValuer valuer);

        /// <summary>Computes a hash code based upon value.</summary>
        /// <param name="valuer">Handles callback behavior for child values.</param>
        /// <returns>The computed hash code.</returns>
        int GetValueHash(IValuer valuer);
    }
}
