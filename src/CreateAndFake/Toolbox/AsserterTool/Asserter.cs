using System;
using System.Collections;
using System.Linq;
using System.Text;
using CreateAndFake.Toolbox.ValuerTool;

namespace CreateAndFake.Toolbox.AsserterTool
{
    /// <summary>Handles common test scenarios.</summary>
    public class Asserter
    {
        /// <summary>Handles comparisons.</summary>
        protected IValuer Valuer { get; }

        /// <summary>Sets up the asserter capabilities.</summary>
        /// <param name="valuer">Handles comparisons.</param>
        /// <exception cref="ArgumentNullException">If given a null valuer.</exception>
        public Asserter(IValuer valuer)
        {
            Valuer = valuer ?? throw new ArgumentNullException(nameof(valuer));
        }

        /// <summary>Verifies two objects are equal by value.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="actual">Object to compare with.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public void Is(object expected, object actual, string details = null)
        {
            ValuesEqual(expected, actual, details);
        }

        /// <summary>Verifies two objects are unequal by value.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="actual">Object to compare with.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public void IsNot(object expected, object actual, string details = null)
        {
            ValuesNotEqual(expected, actual, details);
        }

        /// <summary>Verifies a collection is empty.</summary>
        /// <param name="collection">Collection to check.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void IsEmpty(IEnumerable collection, string details = null)
        {
            HasCount(0, collection, details);
        }

        /// <summary>Verifies a collection is not empty.</summary>
        /// <param name="collection">Collection to check.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void IsNotEmpty(IEnumerable collection, string details = null)
        {
            Is(true, collection?.GetEnumerator().MoveNext(), details);
        }

        /// <summary>Verifies a collection is of a certain size.</summary>
        /// <param name="count">Size to check for.</param>
        /// <param name="collection">Collection to check.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void HasCount(int count, IEnumerable collection, string details = null)
        {
            if (collection == null)
            {
                throw new AssertException("Expected collection of '" +
                    count + "' elements, but was 'null'.", details);
            }

            StringBuilder contents = new StringBuilder();
            int i = 0;
            for (IEnumerator data = collection.GetEnumerator(); data.MoveNext(); i++)
            {
                contents.Append("[").Append(i).Append("]:").Append(data.Current).AppendLine();
            }

            if (i != count)
            {
                throw new AssertException("Expected collection of '" + count +
                    "' elements, but was '" + i + "'.", details, contents.ToString());
            }
        }

        /// <summary>Verifies two objects are equal by reference.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="actual">Object to compare with.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void ReferenceEqual(object expected, object actual, string details = null)
        {
            if (!ReferenceEquals(expected, actual))
            {
                throw new AssertException("References failed to equal.", details);
            }
        }

        /// <summary>Verifies two objects are not equal by reference.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="actual">Object to compare with.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void ReferenceNotEqual(object expected, object actual, string details = null)
        {
            if (ReferenceEquals(expected, actual))
            {
                throw new AssertException("References failed to not equal.", details);
            }
        }

        /// <summary>Verifies two objects are equal by value.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="actual">Object to compare with.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void ValuesEqual(object expected, object actual, string details = null)
        {
            Difference[] differences = Valuer.Compare(expected, actual).ToArray();
            if (differences.Length > 0)
            {
                Type rootType = expected?.GetType() ?? actual.GetType();
                throw new AssertException("Value equality failed for type '" + rootType.Name + "'.",
                    details, string.Join<Difference>(Environment.NewLine, differences));
            }
        }

        /// <summary>Verifies two objects are unequal by value.</summary>
        /// <param name="expected">Object to compare against.</param>
        /// <param name="actual">Object to compare with.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void ValuesNotEqual(object expected, object actual, string details = null)
        {
            if (!Valuer.Compare(expected, actual).Any())
            {
                throw new AssertException("Value inequality failed for type '" +
                    expected?.GetType().Name + "'.", details, expected?.ToString());
            }
        }

        /// <summary>Verifies the given behavior throws an exception.</summary>
        /// <typeparam name="T">Exception type expected.</typeparam>
        /// <param name="behavior">Behavior to verify.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void Throws<T>(Action behavior, string details = null) where T : Exception
        {
            Throws<T>(behavior, null, details);
        }

        /// <summary>Verifies the given behavior throws an exception.</summary>
        /// <typeparam name="T">Exception type expected.</typeparam>
        /// <param name="behavior">Behavior to verify.</param>
        /// <param name="verifier">Extra verification for the exception.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void Throws<T>(Action behavior, Action<T> verifier, string details = null) where T : Exception
        {
            Throws<T>(() => { behavior?.Invoke(); return true; }, verifier, details);
        }

        /// <summary>Verifies the given behavior throws an exception.</summary>
        /// <typeparam name="T">Exception type expected.</typeparam>
        /// <param name="behavior">Behavior to verify.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void Throws<T>(Func<object> behavior, string details = null) where T : Exception
        {
            Throws<T>(behavior, null, details);
        }

        /// <summary>Verifies the given behavior throws an exception.</summary>
        /// <typeparam name="T">Exception type expected.</typeparam>
        /// <param name="behavior">Behavior to verify.</param>
        /// <param name="verifier">Extra verification for the exception.</param>
        /// <param name="details">Optional failure details.</param>
        /// <exception cref="AssertException">If the expected behavior doesn't happen.</exception>
        public virtual void Throws<T>(Func<object> behavior, Action<T> verifier, string details = null) where T : Exception
        {
            string errorMessage = "Expected exception of type '" + typeof(T).FullName + "'.";
            try
            {
                (behavior?.Invoke() as IDisposable)?.Dispose();
            }
            catch (T e)
            {
                verifier?.Invoke(e);
                return;
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions.Count == 1 && e.InnerExceptions[0] is T actual)
                {
                    verifier?.Invoke(actual);
                    return;
                }
                else
                {
                    throw new AssertException(errorMessage, details, e);
                }
            }
            catch (Exception e)
            {
                throw new AssertException(errorMessage, details, e);
            }

            throw new AssertException(errorMessage, details);
        }
    }
}
