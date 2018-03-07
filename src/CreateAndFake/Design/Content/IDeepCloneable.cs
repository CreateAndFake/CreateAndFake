namespace CreateAndFake.Design.Content
{
    /// <summary>Provides deep copying functionality.</summary>
    public interface IDeepCloneable
    {
        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <returns>Clone that is equal in value to the current instance.</returns>
        IDeepCloneable DeepClone();
    }
}
