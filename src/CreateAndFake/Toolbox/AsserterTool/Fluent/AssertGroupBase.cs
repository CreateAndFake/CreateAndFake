using System.Collections;
using System.Text;
using CreateAndFake.Design.Randomization;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls for collections.</summary>
    public abstract class AssertGroupBase<T> : AssertObjectBase<T> where T : AssertGroupBase<T>
    {
        /// <summary>Collection to check.</summary>
        protected IEnumerable Collection { get; }

        /// <summary>Initializes a new instance of the <see cref="AssertGroupBase{T}"/> class.</summary>
        /// <param name="gen">Core value random handler.</param>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="collection">Collection to check.</param>
        protected AssertGroupBase(IRandom gen, IValuer valuer, IEnumerable collection) : base(gen, valuer, collection)
        {
            Collection = collection;
        }

        /// <summary>Verifies the collection is empty.</summary>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If collection has elements.</exception>
        public AssertChainer<T> IsEmpty(string details = null)
        {
            return HasCount(0, details);
        }

        /// <summary>Verifies the collection is not empty.</summary>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If collection is null or has no elements.</exception>
        public AssertChainer<T> IsNotEmpty(string details = null)
        {
            _ = new AssertObject(Gen, Valuer, Collection?.GetEnumerator().MoveNext()).Is(true, details);
            return ToChainer();
        }

        /// <summary>Verifies the collection has <paramref name="count"/> elements.</summary>
        /// <param name="count">Size to check for.</param>
        /// <param name="details">Optional failure details.</param>
        /// <returns>Chainer to make additional assertions with.</returns>
        /// <exception cref="AssertException">If collection size does not match <paramref name="count"/>.</exception>
        public AssertChainer<T> HasCount(int count, string details = null)
        {
            if (Collection == null)
            {
                throw new AssertException(
                    $"Expected collection of '{count}' elements, but was 'null'.", details, Gen.InitialSeed);
            }

            StringBuilder contents = new();
            int i = 0;
            for (IEnumerator data = Collection.GetEnumerator(); data.MoveNext(); i++)
            {
                _ = contents.Append('[').Append(i).Append("]:").Append(data.Current).AppendLine();
            }

            if (i != count)
            {
                throw new AssertException(
                    $"Expected collection of '{count}' elements, but was '{i}'.",
                    details, Gen.InitialSeed, contents.ToString());
            }

            return ToChainer();
        }
    }
}
