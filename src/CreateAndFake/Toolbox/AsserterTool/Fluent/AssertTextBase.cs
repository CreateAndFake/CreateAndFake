using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

#pragma warning disable CA1307 // Specify StringComparison for clarity
#pragma warning disable CA1310 // Specify StringComparison for correctness

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls for strings.</summary>
    public abstract class AssertTextBase<T> : AssertGroupBase<T> where T : AssertTextBase<T>
    {
        /// <summary>Text to check.</summary>
        protected string Text { get; }

        /// <summary>Initializes a new instance of the <see cref="AssertTextBase{T}"/> class.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="text">Text to check.</param>
        protected AssertTextBase(IRandom gen, IValuer valuer, string text) : base(gen, valuer, text)
        {
            Text = text;
        }

        /// <summary>Verifies the text contains <paramref name="content"/>.</summary>
        /// <param name="content">Inner text to check for.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If text does not contain <paramref name="content"/>.</exception>
        public virtual AssertChainer<T> Contains(string content, string details = null)
        {
            if (!Text.Contains(content))
            {
                throw new AssertException($"Text was missing '{details}'.", details, Gen.InitialSeed, Text);
            }
            return ToChainer();
        }

        /// <summary>Verifies the text does not contain <paramref name="content"/>.</summary>
        /// <param name="content">Inner text to check for.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If text contains <paramref name="content"/>.</exception>
        public virtual AssertChainer<T> ContainsNot(string content, string details = null)
        {
            if (Text.Contains(content))
            {
                throw new AssertException($"Text contained '{details}'.", details, Gen.InitialSeed, Text);
            }
            return ToChainer();
        }

        /// <summary>Verifies the text starts with <paramref name="content"/>.</summary>
        /// <param name="content">Inner text to check for.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If text does not start with <paramref name="content"/>.</exception>
        public virtual AssertChainer<T> StartsWith(string content, string details = null)
        {
            if (!Text.StartsWith(content))
            {
                throw new AssertException($"Text did not start with '{details}'.", details, Gen.InitialSeed, Text);
            }
            return ToChainer();
        }

        /// <summary>Verifies the text does not start with <paramref name="content"/>.</summary>
        /// <param name="content">Inner text to check for.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If text starts with <paramref name="content"/>.</exception>
        public virtual AssertChainer<T> StartsNotWith(string content, string details = null)
        {
            if (Text.StartsWith(content))
            {
                throw new AssertException($"Text started with '{details}'.", details, Gen.InitialSeed, Text);
            }
            return ToChainer();
        }

        /// <summary>Verifies the text ends with <paramref name="content"/>.</summary>
        /// <param name="content">Inner text to check for.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If text does not end with <paramref name="content"/>.</exception>
        public virtual AssertChainer<T> EndsWith(string content, string details = null)
        {
            if (!Text.EndsWith(content))
            {
                throw new AssertException($"Text did not end with '{details}'.", details, Gen.InitialSeed, Text);
            }
            return ToChainer();
        }

        /// <summary>Verifies the text does not end with <paramref name="content"/>.</summary>
        /// <param name="content">Inner text to check for.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If text ends with <paramref name="content"/>.</exception>
        public virtual AssertChainer<T> EndsNotWith(string content, string details = null)
        {
            if (Text.EndsWith(content))
            {
                throw new AssertException($"Text ended with '{details}'.", details, Gen.InitialSeed, Text);
            }
            return ToChainer();
        }
    }
}

#pragma warning restore CA1307 // Specify StringComparison for clarity
#pragma warning restore CA1310 // Specify StringComparison for correctness
