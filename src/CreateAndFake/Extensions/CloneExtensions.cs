using CreateAndFake.Toolbox.DuplicatorTool;

namespace CreateAndFake.Fluent
{
    /// <summary>Provides cloning assertions.</summary>
    public static class CloneExtensions
    {
        /// <inheritdoc cref="IDuplicator.Copy{T}(T)"/>
        public static T CreateDeepClone<T>(this T source)
        {
            return Tools.Duplicator.Copy(source);
        }
    }
}
