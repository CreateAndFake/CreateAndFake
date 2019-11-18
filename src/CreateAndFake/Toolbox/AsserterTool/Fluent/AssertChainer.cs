namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Allows assertion calls to be chained.</summary>
    /// <typeparam name="T">Assertion type to chain.</typeparam>
    public sealed class AssertChainer<T>
    {
        /// <summary>Includes another assertion.</summary>
        public T And { get; }

        /// <summary>Initializer.</summary>
        /// <param name="chain">Assertion class to chain.</param>
        public AssertChainer(T chain)
        {
            And = chain;
        }
    }
}
