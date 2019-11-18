using System.Collections;
using System.Text;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool.Fluent
{
    /// <summary>Handles assertion calls.</summary>
    public abstract class AssertCollectionBase<T> : AssertObjectBase<T> where T : AssertCollectionBase<T>
    {
        /// <summary>Collection to check.</summary>
        protected IEnumerable Collection { get; }

        /// <summary>Initializer.</summary>
        /// <param name="valuer">Handles comparisons.</param>
        /// <param name="collection">Collection to check.</param>
        protected AssertCollectionBase(IValuer valuer, IEnumerable collection) : base(valuer, collection)
        {
            Collection = collection;
        }

        /// <summary>Verifies a collection is empty.</summary>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public AssertChainer<T> IsEmpty(string details = null)
        {
            return HasCount(0, details);
        }

        /// <summary>Verifies a collection is not empty.</summary>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public AssertChainer<T> IsNotEmpty(string details = null)
        {
            new AssertObject(Valuer, Collection?.GetEnumerator().MoveNext()).Is(true, details);
            return ToChainer();
        }

        /// <summary>Verifies a collection is of a certain size.</summary>
        /// <param name="count">Size to check for.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        /// <returns>Chainer to make additional assertions with.</returns>
        public AssertChainer<T> HasCount(int count, string details = null)
        {
            if (Collection == null)
            {
                throw new AssertException(
                    $"Expected collection of '{count}' elements, but was 'null'.", details);
            }

            StringBuilder contents = new StringBuilder();
            int i = 0;
            for (IEnumerator data = Collection.GetEnumerator(); data.MoveNext(); i++)
            {
                contents.Append("[").Append(i).Append("]:").Append(data.Current).AppendLine();
            }

            if (i != count)
            {
                throw new AssertException(
                    $"Expected collection of '{count}' elements, but was '{i}'.", details, contents.ToString());
            }

            return ToChainer();
        }
    }
}
