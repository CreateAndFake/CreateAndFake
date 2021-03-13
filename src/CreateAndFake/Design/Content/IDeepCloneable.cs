namespace CreateAndFake.Design.Content
{
    /// <summary>Provides self deep copying functionality.</summary>
    public interface IDeepCloneable
    {
        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <returns>Created clone that is equal in value to the instance.</returns>
        IDeepCloneable DeepClone();
    }
}
