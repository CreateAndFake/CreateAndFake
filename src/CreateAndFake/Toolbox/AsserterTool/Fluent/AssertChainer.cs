using System.Collections;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Allows assertion calls to be chained.</summary>
    /// <typeparam name="T">Assertion type to chain.</typeparam>
    public sealed class AssertChainer<T>
    {
        /// <summary>Core value random handler.</summary>
        private readonly IRandom _gen;

        /// <summary>Handles comparisons.</summary>
        private readonly IValuer _valuer;

        /// <summary>Includes another assertion.</summary>
        public T And { get; }

        /// <summary>Initializer.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="chain">Assertion class to chain.</param>
        /// <param name="valuer">Handles comparisons.</param>
        public AssertChainer(T chain, IRandom gen, IValuer valuer)
        {
            _gen = gen;
            _valuer = valuer;
            And = chain;
        }

        /// <summary>Specifies another value to test.</summary>
        /// <param name="actual">Object to test.</param>
        /// <returns>Asserter to test with.</returns>
        public AssertObject Also(object actual)
        {
            return new AssertObject(_gen, _valuer, actual);
        }

        /// <summary>Specifies another value to test.</summary>
        /// <param name="collection">Collection to test.</param>
        /// <returns>Asserter to test with.</returns>
        public AssertGroup Also(IEnumerable collection)
        {
            return new AssertGroup(_gen, _valuer, collection);
        }
    }
}
