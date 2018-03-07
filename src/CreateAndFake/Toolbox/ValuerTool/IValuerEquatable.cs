namespace CreateAndFake.Toolbox.ValuerTool
{
    /// <summary>Provides value equality with the aid of the valuer.</summary>
    public interface IValuerEquatable
    {
        /// <summary>Compares by value.</summary>
        /// <param name="other">Instance to compare with.</param>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>True if equal; false otherwise.</returns>
        bool ValuesEqual(object other, IValuer valuer);

        /// <summary>Generates a hash based upon value.</summary>
        /// <param name="valuer">Valuer to handle child values.</param>
        /// <returns>The generated hash code.</returns>
        int GetValueHash(IValuer valuer);
    }
}
